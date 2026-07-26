using AetherView.App.Domain.Enums;
using AetherView.App.Features.Home;
using AetherView.App.Services.Projects;

namespace AetherView.Tests;

public sealed class HomeViewModelTests
{
    private static readonly DateTimeOffset BaseTime =
        new(2026, 7, 26, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task LoadAsync_ShowsFiveMostRecentlyUpdatedSessions()
    {
        RecentProjectSummary[] projects = Enumerable
            .Range(0, 7)
            .Select(index => CreateProject(index))
            .ToArray();
        StubRecentProjectService service = new(projects);
        HomeViewModel viewModel = new(service);

        await viewModel.LoadCommand.ExecuteAsync(null);

        Assert.Equal(HomeViewModel.RecentSessionLimit, service.RequestedMaximumCount);
        Assert.Equal(5, viewModel.RecentSessions.Count);
        Assert.Collection(
            viewModel.RecentSessions,
            item => Assert.Equal("Sesja 6", item.Name),
            item => Assert.Equal("Sesja 5", item.Name),
            item => Assert.Equal("Sesja 4", item.Name),
            item => Assert.Equal("Sesja 3", item.Name),
            item => Assert.Equal("Sesja 2", item.Name));
        Assert.False(viewModel.HasError);
        Assert.False(viewModel.IsBusy);
    }

    [Fact]
    public async Task LoadAsync_WithNoSessions_ShowsEmptyCollection()
    {
        StubRecentProjectService service = new([]);
        HomeViewModel viewModel = new(service);

        await viewModel.LoadCommand.ExecuteAsync(null);

        Assert.Empty(viewModel.RecentSessions);
        Assert.False(viewModel.HasError);
        Assert.False(viewModel.IsBusy);
    }

    [Fact]
    public async Task LoadAsync_WhenReadFails_ShowsSafeError()
    {
        FailingRecentProjectService service = new();
        HomeViewModel viewModel = new(service);

        await viewModel.LoadCommand.ExecuteAsync(null);

        Assert.True(viewModel.HasError);
        Assert.Equal(
            "Nie udało się wczytać ostatnich sesji. Spróbuj odświeżyć ekran.",
            viewModel.ErrorMessage);
        Assert.DoesNotContain("database", viewModel.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        Assert.False(viewModel.IsBusy);
    }

    private static RecentProjectSummary CreateProject(int index)
    {
        DateTimeOffset createdAt = BaseTime.AddDays(index);

        return new RecentProjectSummary(
            Guid.NewGuid(),
            $"Sesja {index}",
            $"Pytanie {index}",
            ArvProjectStatus.Draft,
            createdAt.AddHours(1),
            createdAt);
    }

    private sealed class StubRecentProjectService(
        IReadOnlyList<RecentProjectSummary> projects) : IRecentProjectService
    {
        public int? RequestedMaximumCount { get; private set; }

        public Task<IReadOnlyList<RecentProjectSummary>> GetRecentAsync(
            int maximumCount,
            CancellationToken cancellationToken)
        {
            RequestedMaximumCount = maximumCount;

            return Task.FromResult(projects);
        }
    }

    private sealed class FailingRecentProjectService : IRecentProjectService
    {
        public Task<IReadOnlyList<RecentProjectSummary>> GetRecentAsync(
            int maximumCount,
            CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("database path should remain private");
        }
    }
}
