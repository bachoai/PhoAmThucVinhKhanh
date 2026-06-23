using Quan4CulinaryTourism.Mobile.ViewModels;

namespace Quan4CulinaryTourism.Mobile.Views;

public partial class SplashPage : ContentPage
{
    private readonly SplashViewModel _viewModel;

    public SplashPage(SplashViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}
