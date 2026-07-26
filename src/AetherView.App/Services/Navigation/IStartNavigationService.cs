namespace AetherView.App.Services.Navigation;

public interface IStartNavigationService
{
    Task GoToNewSessionAsync(CancellationToken cancellationToken);

    Task GoToHistoryAsync(CancellationToken cancellationToken);

    Task GoToDashboardAsync(CancellationToken cancellationToken);
}
