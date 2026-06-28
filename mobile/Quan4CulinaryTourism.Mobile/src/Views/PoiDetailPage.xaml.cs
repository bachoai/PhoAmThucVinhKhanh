using Quan4CulinaryTourism.Mobile.ViewModels;

namespace Quan4CulinaryTourism.Mobile.Views;

public partial class PoiDetailPage : ContentPage, IQueryAttributable
{
    private readonly PoiDetailViewModel _viewModel;
    private string? _poiId;
    private string? _autoplayMode;
    private string _source = "detail";

    public PoiDetailPage(PoiDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _poiId = query.TryGetValue("id", out var id) ? id?.ToString() : null;
        _autoplayMode = query.TryGetValue("autoplay", out var autoplay) ? autoplay?.ToString() : null;
        _source = query.TryGetValue("source", out var source) && !string.IsNullOrWhiteSpace(source?.ToString())
            ? source!.ToString()!
            : "detail";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.AttachAudioEvents();
        if (!string.IsNullOrWhiteSpace(_poiId))
        {
            await _viewModel.InitializeAsync(_poiId, _autoplayMode, _source);
            _autoplayMode = null;
            _source = "detail";
        }
    }

    protected override void OnDisappearing()
    {
        _viewModel.DetachAudioEvents();
        base.OnDisappearing();
    }
}
