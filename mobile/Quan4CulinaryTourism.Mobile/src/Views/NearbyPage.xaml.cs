using Quan4CulinaryTourism.Mobile.ViewModels;

namespace Quan4CulinaryTourism.Mobile.Views;

public partial class NearbyPage : ContentPage
{
    private readonly NearbyViewModel _viewModel;

    public NearbyPage(NearbyViewModel viewModel)
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
