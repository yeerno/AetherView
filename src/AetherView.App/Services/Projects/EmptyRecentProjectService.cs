namespace AetherView.App.Services.Projects;

public sealed class EmptyRecentProjectService : IRecentProjectService
{
    public Task<IReadOnlyList<RecentProjectSummary>> GetRecentAsync(
        int maximumCount,
        CancellationToken cancellationToken)
    {
        if (maximumCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumCount));
        }

        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<RecentProjectSummary> projects =
            Array.Empty<RecentProjectSummary>();

        return Task.FromResult(projects);
    }
}
