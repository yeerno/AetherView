using CommunityToolkit.Mvvm.ComponentModel;

namespace AetherView.App.Features.Projects;

public sealed partial class ProjectEditorViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Name { get; set; } = "Mecz pokazowy";

    [ObservableProperty]
    public partial string Question { get; set; } =
        "Który zawodnik wygra spotkanie?";

    [ObservableProperty]
    public partial string OutcomeAName { get; set; } = "Zawodnik A";

    [ObservableProperty]
    public partial string OutcomeBName { get; set; } = "Zawodnik B";

    [ObservableProperty]
    public partial int PlannedTrialCount { get; set; } = 8;

    public string EventStartsAtDisplay { get; } = "30.07.2026 · 18:00";

    public string FeedbackAvailableAtDisplay { get; } = "30.07.2026 · 21:30";
}
