using AetherView.App.Domain.Enums;
using AetherView.App.Domain.Results;

namespace AetherView.App.Domain.Entities;

public sealed class ArvTrial
{
    public ArvTrial(Guid id, Guid projectId, int sequenceNumber)
    {
        DomainGuard.AgainstEmpty(id, nameof(id));
        DomainGuard.AgainstEmpty(projectId, nameof(projectId));

        if (sequenceNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sequenceNumber));
        }

        Id = id;
        ProjectId = projectId;
        SequenceNumber = sequenceNumber;
        Status = TrialStatus.NotStarted;
    }

    public Guid Id { get; }

    public Guid ProjectId { get; }

    public int SequenceNumber { get; }

    public TrialStatus Status { get; private set; }

    public DateTimeOffset? StartedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public string? Notes { get; private set; }

    public DomainResult Start(DateTimeOffset startedAt)
    {
        if (Status is not TrialStatus.NotStarted)
        {
            return DomainResult.Failure(DomainError.InvalidTrialStatus);
        }

        StartedAt = startedAt;
        Status = TrialStatus.InProgress;

        return DomainResult.Success();
    }

    public DomainResult Complete(string? notes, DateTimeOffset completedAt)
    {
        if (Status is not TrialStatus.InProgress)
        {
            return DomainResult.Failure(DomainError.InvalidTrialStatus);
        }

        Notes = notes?.Trim();
        CompletedAt = completedAt;
        Status = TrialStatus.Completed;

        return DomainResult.Success();
    }
}
