namespace AetherView.App.Features.History;

public sealed class HistoryViewModel
{
    public IReadOnlyList<HistorySessionItemViewModel> Sessions { get; } =
    [
        new(
            "Test protokołu",
            "24.07.2026 · 16:00",
            "Trafienie",
            "Outcome A",
            "#DCEFEA",
            "#285D59"),
        new(
            "Sesja treningowa 11",
            "21.07.2026 · 19:30",
            "Nietrafienie",
            "Outcome B",
            "#F7DFDB",
            "#84382E"),
        new(
            "Prognoza tygodniowa",
            "18.07.2026 · 20:00",
            "Trafienie",
            "Outcome A",
            "#DCEFEA",
            "#285D59"),
        new(
            "Sesja treningowa 10",
            "14.07.2026 · 18:45",
            "Bez prognozy",
            "Outcome B",
            "#ECEFEE",
            "#52635F")
    ];
}

public sealed record HistorySessionItemViewModel(
    string Name,
    string Date,
    string Result,
    string ActualOutcome,
    string ResultBackground,
    string ResultForeground);
