using AetherView.App.Features.Feedback;
using AetherView.App.Features.Home;
using AetherView.App.Features.Judging;
using AetherView.App.Features.Projects;
using AetherView.App.Features.Session;
using AetherView.App.Features.Settings;
using AetherView.App.Features.Statistics;
using AetherView.App.Services.Clock;
using AetherView.App.Services.Feedback;
using AetherView.App.Services.Projects;
using AetherView.App.Services.Randomization;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace AetherView.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<IClock, SystemClock>();
        builder.Services.AddSingleton<IBlindAssignmentService, BlindAssignmentService>();
        builder.Services.AddSingleton<IFeedbackRevealService, FeedbackRevealService>();
        builder.Services.AddSingleton<IRecentProjectService, PreviewRecentProjectService>();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<ProjectsViewModel>();
        builder.Services.AddTransient<ProjectsPage>();
        builder.Services.AddTransient<ProjectEditorViewModel>();
        builder.Services.AddTransient<ProjectEditorPage>();
        builder.Services.AddTransient<SessionViewModel>();
        builder.Services.AddTransient<SessionPage>();
        builder.Services.AddTransient<JudgingViewModel>();
        builder.Services.AddTransient<JudgingPage>();
        builder.Services.AddTransient<FeedbackViewModel>();
        builder.Services.AddTransient<FeedbackPage>();
        builder.Services.AddTransient<StatisticsViewModel>();
        builder.Services.AddTransient<StatisticsPage>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<SettingsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
