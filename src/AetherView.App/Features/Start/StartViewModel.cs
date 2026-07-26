using AetherView.App.Services.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AetherView.App.Features.Start;

public sealed partial class StartViewModel(
    IStartNavigationService navigationService) : ObservableObject
{
    private readonly IStartNavigationService navigationService =
        navigationService ?? throw new ArgumentNullException(nameof(navigationService));

    [RelayCommand(AllowConcurrentExecutions = false)]
    private Task CreateSessionAsync(CancellationToken cancellationToken)
    {
        return navigationService.GoToNewSessionAsync(cancellationToken);
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private Task OpenHistoryAsync(CancellationToken cancellationToken)
    {
        return navigationService.GoToHistoryAsync(cancellationToken);
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private Task OpenDashboardAsync(CancellationToken cancellationToken)
    {
        return navigationService.GoToDashboardAsync(cancellationToken);
    }
}
