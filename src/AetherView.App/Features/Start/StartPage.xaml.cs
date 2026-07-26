namespace AetherView.App.Features.Start;

public partial class StartPage : ContentPage
{
    public StartPage(StartViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        InitializeComponent();
        BindingContext = viewModel;
    }
}
