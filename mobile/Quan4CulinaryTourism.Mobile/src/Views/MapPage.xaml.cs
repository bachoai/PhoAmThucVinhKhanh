using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Quan4CulinaryTourism.Mobile.ViewModels;

namespace Quan4CulinaryTourism.Mobile.Views;

public partial class MapPage : ContentPage
{
    private readonly MapViewModel _viewModel;
    private Microsoft.Maui.Controls.Maps.Map? _poiMap;

    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
        EnsureMap();
        RenderPins();
    }

    private void EnsureMap()
    {
        if (!_viewModel.IsMapAvailable || _poiMap is not null)
        {
            return;
        }

        _poiMap = new Microsoft.Maui.Controls.Maps.Map();
        MapHost.Content = _poiMap;
    }

    private void RenderPins()
    {
        if (_poiMap is null)
        {
            return;
        }

        _poiMap.Pins.Clear();

        if (_viewModel.UserLocation is not null)
        {
            _poiMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                new Location(_viewModel.UserLocation.Latitude, _viewModel.UserLocation.Longitude),
                Distance.FromKilometers(2)));

            _poiMap.Pins.Add(new Pin
            {
                Label = "Bạn đang ở đây",
                Location = new Location(_viewModel.UserLocation.Latitude, _viewModel.UserLocation.Longitude)
            });
        }

        foreach (var poi in _viewModel.Pois)
        {
            var pin = new Pin
            {
                Label = poi.Name,
                Address = poi.DisplayAddress,
                Location = new Location(poi.Latitude, poi.Longitude)
            };

            pin.MarkerClicked += async (_, _) => await _viewModel.OpenDetailCommand.ExecuteAsync(poi.Id);
            _poiMap.Pins.Add(pin);
        }
    }
}
