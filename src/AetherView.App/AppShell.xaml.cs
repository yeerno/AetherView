using AetherView.App.Features.Feedback;
using AetherView.App.Features.History;
using AetherView.App.Features.Home;
using AetherView.App.Features.Judging;
using AetherView.App.Features.Projects;
using AetherView.App.Features.Session;
using AetherView.App.Features.Settings;
using AetherView.App.Features.Start;
using AetherView.App.Features.Statistics;

namespace AetherView.App;

public partial class AppShell : Shell
{
    public AppShell(
        StartPage startPage,
        HomePage homePage,
        HistoryPage historyPage,
        ProjectsPage projectsPage,
        ProjectEditorPage projectEditorPage,
        SessionPage sessionPage,
        JudgingPage judgingPage,
        FeedbackPage feedbackPage,
        StatisticsPage statisticsPage,
        SettingsPage settingsPage)
    {
        ArgumentNullException.ThrowIfNull(startPage);
        ArgumentNullException.ThrowIfNull(homePage);
        ArgumentNullException.ThrowIfNull(historyPage);
        ArgumentNullException.ThrowIfNull(projectsPage);
        ArgumentNullException.ThrowIfNull(projectEditorPage);
        ArgumentNullException.ThrowIfNull(sessionPage);
        ArgumentNullException.ThrowIfNull(judgingPage);
        ArgumentNullException.ThrowIfNull(feedbackPage);
        ArgumentNullException.ThrowIfNull(statisticsPage);
        ArgumentNullException.ThrowIfNull(settingsPage);

        InitializeComponent();

        StartContent.Content = startPage;
        HomeContent.Content = homePage;
        HistoryContent.Content = historyPage;
        ProjectsContent.Content = projectsPage;
        ProjectEditorContent.Content = projectEditorPage;
        SessionContent.Content = sessionPage;
        JudgingContent.Content = judgingPage;
        FeedbackContent.Content = feedbackPage;
        StatisticsContent.Content = statisticsPage;
        SettingsContent.Content = settingsPage;
    }
}
