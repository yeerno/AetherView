using CommunityToolkit.Mvvm.ComponentModel;

namespace AetherView.App.Features.Judging;

public sealed partial class JudgingViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ConfidenceDisplay))]
    public partial double Confidence { get; set; } = 2.8;

    [ObservableProperty]
    public partial bool FirstImageSelected { get; set; } = true;

    [ObservableProperty]
    public partial bool SecondImageSelected { get; set; }

    public string ProjectName { get; } = "Mecz wieczorny";

    public string TrialCounter { get; } = "Ocena próby 4 z 10";

    public string ConfidenceDisplay => Confidence.ToString("0.0");
}
