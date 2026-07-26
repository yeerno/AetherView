using AetherView.App.Domain.Enums;

namespace AetherView.App.Domain.Entities;

public sealed class BlindAssignment
{
    internal BlindAssignment(
        Guid id,
        Guid projectId,
        Guid warmImageId,
        Guid coldImageId,
        Guid outcomeAImageId,
        Guid outcomeBImageId,
        DateTimeOffset createdAt)
    {
        DomainGuard.AgainstEmpty(id, nameof(id));
        DomainGuard.AgainstEmpty(projectId, nameof(projectId));
        DomainGuard.AgainstEmpty(warmImageId, nameof(warmImageId));
        DomainGuard.AgainstEmpty(coldImageId, nameof(coldImageId));
        DomainGuard.AgainstEmpty(outcomeAImageId, nameof(outcomeAImageId));
        DomainGuard.AgainstEmpty(outcomeBImageId, nameof(outcomeBImageId));

        Id = id;
        ProjectId = projectId;
        WarmImageId = warmImageId;
        ColdImageId = coldImageId;
        OutcomeAImageId = outcomeAImageId;
        OutcomeBImageId = outcomeBImageId;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }

    public Guid ProjectId { get; }

    public DateTimeOffset CreatedAt { get; }

    public bool IsLocked => true;

    internal Guid WarmImageId { get; }

    internal Guid ColdImageId { get; }

    internal Guid OutcomeAImageId { get; }

    internal Guid OutcomeBImageId { get; }

    internal bool IsValidFor(Guid projectId)
    {
        if (!IsLocked
            || ProjectId != projectId
            || WarmImageId == ColdImageId
            || OutcomeAImageId == OutcomeBImageId)
        {
            return false;
        }

        return (OutcomeAImageId == WarmImageId && OutcomeBImageId == ColdImageId)
            || (OutcomeAImageId == ColdImageId && OutcomeBImageId == WarmImageId);
    }

    internal Guid ResolveImageId(ActualOutcome actualOutcome)
    {
        return actualOutcome switch
        {
            ActualOutcome.OutcomeA => OutcomeAImageId,
            ActualOutcome.OutcomeB => OutcomeBImageId,
            _ => Guid.Empty
        };
    }
}
