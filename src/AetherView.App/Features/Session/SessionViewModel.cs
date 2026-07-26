using CommunityToolkit.Mvvm.ComponentModel;

namespace AetherView.App.Features.Session;

public sealed partial class SessionViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Notes { get; set; } =
        "Lekki ruch w lewo, chłodna powierzchnia, wrażenie otwartej przestrzeni…";

    public string ProjectName { get; } = "Sesja treningowa 12";

    public string TrialCounter { get; } = "Próba 3 z 8";

    public string ElapsedTime { get; } = "04:32";

    public double Progress { get; } = 0.375;
}
