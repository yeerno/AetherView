using CommunityToolkit.Mvvm.ComponentModel;

namespace AetherView.App.Features.Settings;

public sealed partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    public partial bool NotificationsEnabled { get; set; } = true;

    [ObservableProperty]
    public partial bool HapticsEnabled { get; set; } = true;

    [ObservableProperty]
    public partial bool ConfirmBeforeReveal { get; set; } = true;

    public string ThemeName { get; } = "Zgodny z systemem";

    public string ProtocolVersion { get; } = "protocol-v1";

    public string ScoringVersion { get; } = "scoring-v1";
}
