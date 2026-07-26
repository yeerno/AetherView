using System.Globalization;
using AetherView.App.Domain.Enums;
using AetherView.App.Services.Projects;

namespace AetherView.App.Features.Home;

public sealed class RecentSessionItemViewModel
{
    public RecentSessionItemViewModel(RecentProjectSummary project)
    {
        ArgumentNullException.ThrowIfNull(project);

        Id = project.Id;
        Name = project.Name;
        Question = project.Question;
        Status = GetStatusDisplayName(project.Status);
        EventTime = $"Wydarzenie: {project.EventStartsAt.ToString(
            "dd.MM.yyyy · HH:mm",
            CultureInfo.CurrentCulture)}";
        UpdatedAt = project.UpdatedAt;
    }

    public Guid Id { get; }

    public string Name { get; }

    public string Question { get; }

    public string Status { get; }

    public string EventTime { get; }

    public DateTimeOffset UpdatedAt { get; }

    private static string GetStatusDisplayName(ArvProjectStatus status)
    {
        return status switch
        {
            ArvProjectStatus.Draft => "Szkic",
            ArvProjectStatus.ProtocolLocked => "Protokół zablokowany",
            ArvProjectStatus.TrialsInProgress => "Próby w toku",
            ArvProjectStatus.TrialsCompleted => "Próby zakończone",
            ArvProjectStatus.JudgingInProgress => "Ocena w toku",
            ArvProjectStatus.PredictionCalculated => "Prognoza gotowa",
            ArvProjectStatus.AwaitingEvent => "Oczekiwanie na wydarzenie",
            ArvProjectStatus.AwaitingFeedback => "Oczekiwanie na feedback",
            ArvProjectStatus.FeedbackRevealed => "Feedback ujawniony",
            ArvProjectStatus.Completed => "Zakończona",
            ArvProjectStatus.Cancelled => "Anulowana",
            ArvProjectStatus.Invalidated => "Unieważniona",
            _ => "Nieznany status"
        };
    }
}
