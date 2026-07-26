using AetherView.App.Features.Feedback;
using AetherView.App.Features.Home;
using AetherView.App.Features.Judging;
using AetherView.App.Features.Projects;
using AetherView.App.Features.Session;
using AetherView.App.Features.Settings;
using AetherView.App.Features.Statistics;

namespace AetherView.App;

public partial class AppShell : Shell
{
    public AppShell(
        HomePage homePage,
        ProjectsPage projectsPage,
        ProjectEditorPage projectEditorPage,
        SessionPage sessionPage,
        JudgingPage judgingPage,
        FeedbackPage feedbackPage,
        StatisticsPage statisticsPage,
        SettingsPage settingsPage)
    {
        ArgumentNullException.ThrowIfNull(homePage);
        ArgumentNullException.ThrowIfNull(projectsPage);
        ArgumentNullException.ThrowIfNull(projectEditorPage);
        ArgumentNullException.ThrowIfNull(sessionPage);
        ArgumentNullException.ThrowIfNull(judgingPage);
        ArgumentNullException.ThrowIfNull(feedbackPage);
        ArgumentNullException.ThrowIfNull(statisticsPage);
        ArgumentNullException.ThrowIfNull(settingsPage);

        InitializeComponent();

        HomeContent.Content = homePage;
        ProjectsContent.Content = projectsPage;
        ProjectEditorContent.Content = projectEditorPage;
        SessionContent.Content = sessionPage;
        JudgingContent.Content = judgingPage;
        FeedbackContent.Content = feedbackPage;
        StatisticsContent.Content = statisticsPage;
        SettingsContent.Content = settingsPage;
    }
}
