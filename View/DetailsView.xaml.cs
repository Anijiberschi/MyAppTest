namespace MyApp.View;

public partial class DetailsView : ContentPage
{
    DetailsViewModel viewModel;

    public DetailsView(DetailsViewModel viewModel)
    {
        this.viewModel = viewModel;
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        viewModel.RefreshPage();
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);

        viewModel.ClosePage();
    }

    private async void MyAnimatedButton_Clicked(object sender, EventArgs e)
    {
        // Animation simple pour le bouton
        var button = sender as Button;
        if (button != null)
        {
            await button.ScaleTo(0.9, 100);
            await button.ScaleTo(1, 100);
        }
    }
}