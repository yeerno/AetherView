using AetherView.App.Features.Start;
using AetherView.App.Services.Navigation;

namespace AetherView.Tests;

public sealed class StartViewModelTests
{
    [Fact]
    public async Task NavigationCommands_OpenExpectedDestinations()
    {
        RecordingStartNavigationService navigationService = new();
        StartViewModel viewModel = new(navigationService);

        await viewModel.CreateSessionCommand.ExecuteAsync(null);
        await viewModel.OpenHistoryCommand.ExecuteAsync(null);
        await viewModel.OpenDashboardCommand.ExecuteAsync(null);

        Assert.Equal(1, navigationService.NewSessionNavigationCount);
        Assert.Equal(1, navigationService.HistoryNavigationCount);
        Assert.Equal(1, navigationService.DashboardNavigationCount);
    }

    private sealed class RecordingStartNavigationService : IStartNavigationService
    {
        public int NewSessionNavigationCount { get; private set; }

        public int HistoryNavigationCount { get; private set; }

        public int DashboardNavigationCount { get; private set; }

        public Task GoToNewSessionAsync(CancellationToken cancellationToken)
        {
            NewSessionNavigationCount++;

            return Task.CompletedTask;
        }

        public Task GoToHistoryAsync(CancellationToken cancellationToken)
        {
            HistoryNavigationCount++;

            return Task.CompletedTask;
        }

        public Task GoToDashboardAsync(CancellationToken cancellationToken)
        {
            DashboardNavigationCount++;

            return Task.CompletedTask;
        }
    }
}
