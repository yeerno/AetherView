namespace AetherView.App.Features.Statistics;

public sealed class StatisticsViewModel
{
    public IReadOnlyList<StatisticMetricViewModel> Metrics { get; } =
    [
        new("12", "Zakończone"),
        new("7", "Trafienia"),
        new("4", "Nietrafienia"),
        new("1", "Bez prognozy")
    ];

    public IReadOnlyList<RecentResultViewModel> RecentResults { get; } =
    [
        new("Finał turnieju", "Trafienie", "24.07.2026", true),
        new("Sesja treningowa 11", "Nietrafienie", "21.07.2026", false),
        new("Prognoza tygodniowa", "Trafienie", "18.07.2026", true)
    ];

    public string HitRate { get; } = "63,6%";
}

public sealed record StatisticMetricViewModel(string Value, string Label);

public sealed record RecentResultViewModel(
    string Name,
    string Result,
    string Date,
    bool IsHit);
