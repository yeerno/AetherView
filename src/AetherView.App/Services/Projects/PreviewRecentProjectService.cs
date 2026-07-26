using AetherView.App.Domain.Enums;

namespace AetherView.App.Services.Projects;

public sealed class PreviewRecentProjectService : IRecentProjectService
{
    private static readonly DateTimeOffset PreviewDate =
        new(2026, 7, 26, 12, 0, 0, TimeSpan.FromHours(2));

    private static readonly IReadOnlyList<RecentProjectSummary> Projects =
    [
        CreateSummary(
            "Finał turnieju",
            "Czy zawodnik A wygra finał?",
            ArvProjectStatus.AwaitingFeedback,
            0,
            0),
        CreateSummary(
            "Mecz wieczorny",
            "Która drużyna zwycięży?",
            ArvProjectStatus.JudgingInProgress,
            1,
            -1),
        CreateSummary(
            "Sesja treningowa 12",
            "Który z dwóch wyników wystąpi?",
            ArvProjectStatus.TrialsInProgress,
            2,
            -2),
        CreateSummary(
            "Prognoza tygodniowa",
            "Czy Outcome A zostanie potwierdzony?",
            ArvProjectStatus.PredictionCalculated,
            3,
            -3),
        CreateSummary(
            "Test protokołu",
            "Który wynik będzie zgodny ze zdarzeniem?",
            ArvProjectStatus.Completed,
            4,
            -4),
        CreateSummary(
            "Archiwalna sesja",
            "Który wynik wystąpił?",
            ArvProjectStatus.Completed,
            5,
            -5)
    ];

    public Task<IReadOnlyList<RecentProjectSummary>> GetRecentAsync(
        int maximumCount,
        CancellationToken cancellationToken)
    {
        if (maximumCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumCount));
        }

        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<RecentProjectSummary> projects = Projects
            .OrderByDescending(project => project.UpdatedAt)
            .Take(maximumCount)
            .ToArray();

        return Task.FromResult(projects);
    }

    private static RecentProjectSummary CreateSummary(
        string name,
        string question,
        ArvProjectStatus status,
        int eventDayOffset,
        int updateDayOffset)
    {
        return new RecentProjectSummary(
            Guid.NewGuid(),
            name,
            question,
            status,
            PreviewDate.AddDays(eventDayOffset),
            PreviewDate.AddDays(updateDayOffset));
    }
}
