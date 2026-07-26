using AetherView.App.Domain.Enums;

namespace AetherView.App.Domain.Entities;

public sealed class PredictionResult
{
    public PredictionResult(
        Guid id,
        Guid projectId,
        decimal outcomeAPoints,
        decimal outcomeBPoints,
        PredictionDecision decision,
        decimal signalDifference,
        string algorithmVersion,
        DateTimeOffset calculatedAt)
    {
        DomainGuard.AgainstEmpty(id, nameof(id));
        DomainGuard.AgainstEmpty(projectId, nameof(projectId));

        if (!Enum.IsDefined(decision))
        {
            throw new ArgumentOutOfRangeException(nameof(decision));
        }

        Id = id;
        ProjectId = projectId;
        OutcomeAPoints = outcomeAPoints;
        OutcomeBPoints = outcomeBPoints;
        Decision = decision;
        SignalDifference = signalDifference;
        AlgorithmVersion = DomainGuard.RequiredText(algorithmVersion, nameof(algorithmVersion));
        CalculatedAt = calculatedAt;
    }

    public Guid Id { get; }

    public Guid ProjectId { get; }

    public decimal OutcomeAPoints { get; }

    public decimal OutcomeBPoints { get; }

    public PredictionDecision Decision { get; }

    public decimal SignalDifference { get; }

    public string AlgorithmVersion { get; }

    public DateTimeOffset CalculatedAt { get; }
}
