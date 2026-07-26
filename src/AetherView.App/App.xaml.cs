using Microsoft.Extensions.DependencyInjection;

namespace AetherView.App;

public partial class App : Application
{
    private readonly IServiceProvider services;

    public App(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);

        InitializeComponent();

        this.services = services;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        AppShell appShell = services.GetRequiredService<AppShell>();

        return new Window(appShell);
    }
}
