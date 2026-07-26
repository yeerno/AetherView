using System.Collections.ObjectModel;
using AetherView.App.Services.Projects;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AetherView.App.Features.Home;

public partial class HomeViewModel : ObservableObject
{
    internal const int RecentSessionLimit = 5;

    private readonly IRecentProjectService recentProjectService;

    public HomeViewModel(IRecentProjectService recentProjectService)
    {
        ArgumentNullException.ThrowIfNull(recentProjectService);

        this.recentProjectService = recentProjectService;
    }

    public ObservableCollection<RecentSessionItemViewModel> RecentSessions { get; } = [];

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    public partial string? ErrorMessage { get; set; }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            IReadOnlyList<RecentProjectSummary> projects =
                await recentProjectService.GetRecentAsync(
                    RecentSessionLimit,
                    cancellationToken);

            RecentSessionItemViewModel[] recentSessions = projects
                .OrderByDescending(project => project.UpdatedAt)
                .Take(RecentSessionLimit)
                .Select(project => new RecentSessionItemViewModel(project))
                .ToArray();

            RecentSessions.Clear();

            foreach (RecentSessionItemViewModel session in recentSessions)
            {
                RecentSessions.Add(session);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch
        {
            ErrorMessage =
                "Nie udało się wczytać ostatnich sesji. Spróbuj odświeżyć ekran.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
