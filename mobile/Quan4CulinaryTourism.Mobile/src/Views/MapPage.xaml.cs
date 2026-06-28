using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Quan4CulinaryTourism.Mobile.ViewModels;

namespace Quan4CulinaryTourism.Mobile.Views;

public partial class MapPage : ContentPage
{
    private readonly MapViewModel _viewModel;
    private Microsoft.Maui.Controls.Maps.Map? _poiMap;
    private WebView? _offlineMapView;

    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        _viewModel.Pois.CollectionChanged += OnPoisCollectionChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
        RenderCurrentState();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (_offlineMapView is not null)
        {
            _offlineMapView.Source = null;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(MapViewModel.UseOfflineMap) or nameof(MapViewModel.OfflineMapHtmlPath) or nameof(MapViewModel.UserLocation))
        {
            MainThread.BeginInvokeOnMainThread(RenderCurrentState);
        }
    }

    private void OnPoisCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        MainThread.BeginInvokeOnMainThread(RenderCurrentState);

    private void RenderCurrentState()
    {
        if (_viewModel.UseOfflineMap && !string.IsNullOrWhiteSpace(_viewModel.OfflineMapHtmlPath))
        {
            EnsureOfflineMap();
            return;
        }

        EnsureNativeMap();
        RenderPins();
    }

    private void EnsureNativeMap()
    {
        if (!_viewModel.IsMapAvailable)
        {
            if (!_viewModel.UseOfflineMap)
            {
                MapHost.Content = null;
            }

            return;
        }

        if (_poiMap is null)
        {
            _poiMap = new Microsoft.Maui.Controls.Maps.Map();
        }

        if (!ReferenceEquals(MapHost.Content, _poiMap))
        {
            MapHost.Content = _poiMap;
        }
    }

    private void EnsureOfflineMap()
    {
        var path = _viewModel.OfflineMapHtmlPath;
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            return;
        }

        _offlineMapView ??= new WebView();
        if (!ReferenceEquals(MapHost.Content, _offlineMapView))
        {
            MapHost.Content = _offlineMapView;
        }

        var basePath = Path.GetDirectoryName(path) ?? FileSystem.AppDataDirectory;
        _offlineMapView.Source = new HtmlWebViewSource
        {
            Html = File.ReadAllText(path),
            BaseUrl = basePath
        };
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
                Label = "Ban dang o day",
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
