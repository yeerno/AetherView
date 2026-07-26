using AetherView.App.Domain.Enums;
using AetherView.App.Domain.Results;

namespace AetherView.App.Domain.Entities;

public sealed class ArvProject
{
    private BlindAssignment? blindAssignment;

    public ArvProject(
        Guid id,
        string name,
        string question,
        string outcomeAName,
        string outcomeBName,
        DateTimeOffset eventStartsAt,
        DateTimeOffset feedbackAvailableAt,
        int plannedTrialCount,
        string protocolVersion,
        string scoringAlgorithmVersion,
        DateTimeOffset createdAt)
    {
        DomainGuard.AgainstEmpty(id, nameof(id));

        string validatedOutcomeAName = DomainGuard.RequiredText(outcomeAName, nameof(outcomeAName));
        string validatedOutcomeBName = DomainGuard.RequiredText(outcomeBName, nameof(outcomeBName));

        if (string.Equals(
            validatedOutcomeAName,
            validatedOutcomeBName,
            StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("The two outcomes must be distinct.", nameof(outcomeBName));
        }

        if (feedbackAvailableAt <= eventStartsAt)
        {
            throw new ArgumentOutOfRangeException(
                nameof(feedbackAvailableAt),
                "Feedback must become available after the event starts.");
        }

        if (plannedTrialCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(plannedTrialCount));
        }

        Id = id;
        Name = DomainGuard.RequiredText(name, nameof(name));
        Question = DomainGuard.RequiredText(question, nameof(question));
        OutcomeAName = validatedOutcomeAName;
        OutcomeBName = validatedOutcomeBName;
        EventStartsAt = eventStartsAt;
        FeedbackAvailableAt = feedbackAvailableAt;
        PlannedTrialCount = plannedTrialCount;
        ProtocolVersion = DomainGuard.RequiredText(protocolVersion, nameof(protocolVersion));
        ScoringAlgorithmVersion = DomainGuard.RequiredText(
            scoringAlgorithmVersion,
            nameof(scoringAlgorithmVersion));
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
        Status = ArvProjectStatus.Draft;
    }

    public Guid Id { get; }

    public string Name { get; }

    public string Question { get; }

    public string OutcomeAName { get; }

    public string OutcomeBName { get; }

    public DateTimeOffset EventStartsAt { get; }

    public DateTimeOffset FeedbackAvailableAt { get; }

    public int PlannedTrialCount { get; }

    public ArvProjectStatus Status { get; private set; }

    public string ProtocolVersion { get; }

    public string ScoringAlgorithmVersion { get; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public PredictionResult? PredictionResult { get; private set; }

    public FeedbackResult? FeedbackResult { get; private set; }

    public bool HasLockedAssignment => blindAssignment is { IsLocked: true };

    internal BlindAssignment? ProtectedAssignment => blindAssignment;

    public DomainResult LockProtocol(BlindAssignment assignment, DateTimeOffset changedAt)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        if (blindAssignment is not null)
        {
            return DomainResult.Failure(DomainError.AssignmentLocked);
        }

        if (Status is not ArvProjectStatus.Draft)
        {
            return DomainResult.Failure(DomainError.InvalidProjectStatus);
        }

        if (assignment.ProjectId != Id)
        {
            return DomainResult.Failure(DomainError.AssignmentProjectMismatch);
        }

        if (!assignment.IsValidFor(Id))
        {
            return DomainResult.Failure(DomainError.InvalidAssignment);
        }

        blindAssignment = assignment;

        return Transition(
            ArvProjectStatus.Draft,
            ArvProjectStatus.ProtocolLocked,
            changedAt);
    }

    public DomainResult StartTrials(DateTimeOffset changedAt)
    {
        return Transition(
            ArvProjectStatus.ProtocolLocked,
            ArvProjectStatus.TrialsInProgress,
            changedAt);
    }

    public DomainResult CompleteTrials(DateTimeOffset changedAt)
    {
        return Transition(
            ArvProjectStatus.TrialsInProgress,
            ArvProjectStatus.TrialsCompleted,
            changedAt);
    }

    public DomainResult StartJudging(DateTimeOffset changedAt)
    {
        return Transition(
            ArvProjectStatus.TrialsCompleted,
            ArvProjectStatus.JudgingInProgress,
            changedAt);
    }

    public DomainResult RecordPrediction(
        PredictionResult predictionResult,
        DateTimeOffset changedAt)
    {
        ArgumentNullException.ThrowIfNull(predictionResult);

        if (Status is not ArvProjectStatus.JudgingInProgress)
        {
            return DomainResult.Failure(DomainError.InvalidProjectStatus);
        }

        if (predictionResult.ProjectId != Id
            || !string.Equals(
                predictionResult.AlgorithmVersion,
                ScoringAlgorithmVersion,
                StringComparison.Ordinal))
        {
            return DomainResult.Failure(DomainError.InvalidPrediction);
        }

        PredictionResult = predictionResult;

        return Transition(
            ArvProjectStatus.JudgingInProgress,
            ArvProjectStatus.PredictionCalculated,
            changedAt);
    }

    public DomainResult MarkAwaitingEvent(DateTimeOffset changedAt)
    {
        return Transition(
            ArvProjectStatus.PredictionCalculated,
            ArvProjectStatus.AwaitingEvent,
            changedAt);
    }

    public DomainResult MarkAwaitingFeedback(DateTimeOffset changedAt)
    {
        return Transition(
            ArvProjectStatus.AwaitingEvent,
            ArvProjectStatus.AwaitingFeedback,
            changedAt);
    }

    public DomainResult Complete(DateTimeOffset changedAt)
    {
        return Transition(
            ArvProjectStatus.FeedbackRevealed,
            ArvProjectStatus.Completed,
            changedAt);
    }

    internal DomainResult RecordFeedback(
        FeedbackResult feedbackResult,
        DateTimeOffset changedAt)
    {
        ArgumentNullException.ThrowIfNull(feedbackResult);

        if (FeedbackResult is not null)
        {
            return DomainResult.Failure(DomainError.FeedbackAlreadyRevealed);
        }

        if (feedbackResult.ProjectId != Id)
        {
            return DomainResult.Failure(DomainError.InvalidProjectStatus);
        }

        DomainResult transitionResult = Transition(
            ArvProjectStatus.AwaitingFeedback,
            ArvProjectStatus.FeedbackRevealed,
            changedAt);

        if (!transitionResult.IsSuccess)
        {
            return transitionResult;
        }

        FeedbackResult = feedbackResult;

        return DomainResult.Success();
    }

    private DomainResult Transition(
        ArvProjectStatus expectedStatus,
        ArvProjectStatus newStatus,
        DateTimeOffset changedAt)
    {
        if (Status != expectedStatus)
        {
            return DomainResult.Failure(DomainError.InvalidProjectStatus);
        }

        Status = newStatus;
        UpdatedAt = changedAt;

        return DomainResult.Success();
    }
}
