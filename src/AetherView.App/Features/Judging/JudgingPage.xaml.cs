namespace AetherView.App.Features.Judging;

public partial class JudgingPage : ContentPage
{
    public JudgingPage(JudgingViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        InitializeComponent();
        BindingContext = viewModel;
    }
}
