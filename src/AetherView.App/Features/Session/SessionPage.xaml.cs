namespace AetherView.App.Features.Session;

public partial class SessionPage : ContentPage
{
    public SessionPage(SessionViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        InitializeComponent();
        BindingContext = viewModel;
    }
}
