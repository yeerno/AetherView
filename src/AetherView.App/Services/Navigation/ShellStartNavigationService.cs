namespace AetherView.App.Services.Navigation;

public sealed class ShellStartNavigationService : IStartNavigationService
{
    public Task GoToNewSessionAsync(CancellationToken cancellationToken)
    {
        return NavigateAsync("//Projects/ProjectEditorPage", cancellationToken);
    }

    public Task GoToHistoryAsync(CancellationToken cancellationToken)
    {
        return NavigateAsync("//History/HistoryPage", cancellationToken);
    }

    public Task GoToDashboardAsync(CancellationToken cancellationToken)
    {
        return NavigateAsync("//Dashboard/HomePage", cancellationToken);
    }

    private static Task NavigateAsync(
        string route,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Shell shell = Shell.Current
            ?? throw new InvalidOperationException("Navigation is not available.");

        return shell.GoToAsync(route);
    }
}
