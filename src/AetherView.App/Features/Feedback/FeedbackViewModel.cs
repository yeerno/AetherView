namespace AetherView.App.Features.Feedback;

public sealed class FeedbackViewModel
{
    public string ProjectName { get; } = "Finał turnieju";

    public string Question { get; } = "Czy zawodnik A wygra finał?";

    public string OutcomeAName { get; } = "Zawodnik A";

    public string OutcomeBName { get; } = "Zawodnik B";

    public string FeedbackAvailableAt { get; } = "Feedback dostępny od 20:30";

    public bool IsFeedbackRevealed { get; } = false;
}
