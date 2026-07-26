using AetherView.App.Domain.Enums;

namespace AetherView.App.Domain.Entities;

public sealed class FeedbackResult
{
    internal FeedbackResult(
        Guid id,
        Guid projectId,
        ActualOutcome actualOutcome,
        Guid revealedImageId,
        DateTimeOffset availableAt,
        DateTimeOffset revealedAt,
        bool? wasPredictionCorrect)
    {
        DomainGuard.AgainstEmpty(id, nameof(id));
        DomainGuard.AgainstEmpty(projectId, nameof(projectId));
        DomainGuard.AgainstEmpty(revealedImageId, nameof(revealedImageId));

        if (actualOutcome is not (ActualOutcome.OutcomeA or ActualOutcome.OutcomeB))
        {
            throw new ArgumentOutOfRangeException(nameof(actualOutcome));
        }

        Id = id;
        ProjectId = projectId;
        ActualOutcome = actualOutcome;
        RevealedImageId = revealedImageId;
        AvailableAt = availableAt;
        RevealedAt = revealedAt;
        WasPredictionCorrect = wasPredictionCorrect;
    }

    public Guid Id { get; }

    public Guid ProjectId { get; }

    public ActualOutcome ActualOutcome { get; }

    public Guid RevealedImageId { get; }

    public DateTimeOffset AvailableAt { get; }

    public DateTimeOffset RevealedAt { get; }

    public bool? WasPredictionCorrect { get; }
}
