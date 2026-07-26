using AetherView.App.Domain.Entities;
using AetherView.App.Domain.Enums;
using AetherView.App.Domain.Results;
using AetherView.App.Services.Clock;

namespace AetherView.App.Services.Feedback;

public sealed class FeedbackRevealService(IClock clock) : IFeedbackRevealService
{
    private readonly IClock clock = clock ?? throw new ArgumentNullException(nameof(clock));

    public DomainResult<FeedbackResult> Reveal(
        ArvProject project,
        ActualOutcome actualOutcome)
    {
        ArgumentNullException.ThrowIfNull(project);

        if (project.FeedbackResult is not null)
        {
            return DomainResult<FeedbackResult>.Failure(DomainError.FeedbackAlreadyRevealed);
        }

        if (project.Status is not ArvProjectStatus.AwaitingFeedback)
        {
            return DomainResult<FeedbackResult>.Failure(DomainError.InvalidProjectStatus);
        }

        DateTimeOffset revealedAt = clock.UtcNow;

        if (revealedAt < project.FeedbackAvailableAt)
        {
            return DomainResult<FeedbackResult>.Failure(DomainError.FeedbackNotAvailable);
        }

        if (actualOutcome is not (ActualOutcome.OutcomeA or ActualOutcome.OutcomeB))
        {
            return DomainResult<FeedbackResult>.Failure(DomainError.ActualOutcomeRequired);
        }

        BlindAssignment? assignment = project.ProtectedAssignment;

        if (assignment is null || !assignment.IsValidFor(project.Id))
        {
            return DomainResult<FeedbackResult>.Failure(DomainError.InvalidAssignment);
        }

        Guid revealedImageId = assignment.ResolveImageId(actualOutcome);

        if (revealedImageId == Guid.Empty)
        {
            return DomainResult<FeedbackResult>.Failure(DomainError.InvalidAssignment);
        }

        FeedbackResult feedbackResult = new(
            Guid.NewGuid(),
            project.Id,
            actualOutcome,
            revealedImageId,
            project.FeedbackAvailableAt,
            revealedAt,
            ResolvePredictionAccuracy(project.PredictionResult, actualOutcome));

        DomainResult recordResult = project.RecordFeedback(feedbackResult, revealedAt);

        if (!recordResult.IsSuccess)
        {
            return DomainResult<FeedbackResult>.Failure(recordResult.Error);
        }

        return DomainResult<FeedbackResult>.Success(feedbackResult);
    }

    private static bool? ResolvePredictionAccuracy(
        PredictionResult? predictionResult,
        ActualOutcome actualOutcome)
    {
        return predictionResult?.Decision switch
        {
            PredictionDecision.OutcomeA => actualOutcome is ActualOutcome.OutcomeA,
            PredictionDecision.OutcomeB => actualOutcome is ActualOutcome.OutcomeB,
            _ => null
        };
    }
}
