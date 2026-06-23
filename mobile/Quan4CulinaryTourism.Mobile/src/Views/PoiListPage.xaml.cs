using Quan4CulinaryTourism.Mobile.ViewModels;

namespace Quan4CulinaryTourism.Mobile.Views;

public partial class PoiListPage : ContentPage, IQueryAttributable
{
    private readonly PoiListViewModel _viewModel;
    private string? _keyword;

    public PoiListPage(PoiListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _keyword = query.TryGetValue("keyword", out var keyword) ? Uri.UnescapeDataString(keyword?.ToString() ?? string.Empty) : null;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync(_keyword);
    }
}
