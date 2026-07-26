namespace AetherView.App.Features.Projects;

public partial class ProjectEditorPage : ContentPage
{
    public ProjectEditorPage(ProjectEditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        InitializeComponent();
        BindingContext = viewModel;
    }
}
