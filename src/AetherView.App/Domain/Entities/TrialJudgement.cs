namespace AetherView.App.Domain.Entities;

public sealed class TrialJudgement
{
    public TrialJudgement(
        Guid id,
        Guid trialId,
        Guid selectedImageId,
        decimal confidence,
        DateTimeOffset judgedAt,
        string? notes = null)
    {
        DomainGuard.AgainstEmpty(id, nameof(id));
        DomainGuard.AgainstEmpty(trialId, nameof(trialId));
        DomainGuard.AgainstEmpty(selectedImageId, nameof(selectedImageId));

        if (confidence is < 0.1m or > 4.0m)
        {
            throw new ArgumentOutOfRangeException(nameof(confidence));
        }

        Id = id;
        TrialId = trialId;
        SelectedImageId = selectedImageId;
        Confidence = confidence;
        JudgedAt = judgedAt;
        Notes = notes?.Trim();
    }

    public Guid Id { get; }

    public Guid TrialId { get; }

    public Guid SelectedImageId { get; }

    public decimal Confidence { get; }

    public DateTimeOffset JudgedAt { get; }

    public string? Notes { get; }
}
