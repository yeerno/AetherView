namespace AetherView.App.Services.Projects;

public interface IRecentProjectService
{
    Task<IReadOnlyList<RecentProjectSummary>> GetRecentAsync(
        int maximumCount,
        CancellationToken cancellationToken);
}
