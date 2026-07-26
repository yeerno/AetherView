namespace AetherView.App.Features.Projects;

public sealed class ProjectsViewModel
{
    public IReadOnlyList<ProjectListItemViewModel> Projects { get; } =
    [
        new(
            "Finał turnieju",
            "Oczekiwanie na feedback",
            "Dzisiaj, 20:30",
            "12 / 12 prób",
            "#DCEFEA",
            "#285D59"),
        new(
            "Mecz wieczorny",
            "Ocena w toku",
            "Jutro, 18:00",
            "8 / 10 prób",
            "#E8E6F5",
            "#51497A"),
        new(
            "Sesja treningowa 12",
            "Próby w toku",
            "29.07, 12:00",
            "3 / 8 prób",
            "#FFF0D8",
            "#775719"),
        new(
            "Prognoza tygodniowa",
            "Prognoza gotowa",
            "31.07, 21:00",
            "6 / 6 prób",
            "#DFECF7",
            "#355E7C"),
        new(
            "Test protokołu",
            "Zakończona",
            "24.07, 16:00",
            "4 / 4 próby",
            "#ECEFEE",
            "#52635F")
    ];
}

public sealed record ProjectListItemViewModel(
    string Name,
    string Status,
    string EventTime,
    string TrialProgress,
    string StatusBackground,
    string StatusForeground);
