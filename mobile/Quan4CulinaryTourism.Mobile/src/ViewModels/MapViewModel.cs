using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;
using Quan4CulinaryTourism.Mobile.Config;
using Quan4CulinaryTourism.Mobile.Models;
using Quan4CulinaryTourism.Mobile.Services;
using System.Collections.ObjectModel;

namespace Quan4CulinaryTourism.Mobile.ViewModels;

public partial class MapViewModel : BaseViewModel
{
    private readonly PoiApiService _poiApiService;
    private readonly LocationService _locationService;
    private readonly SettingsService _settingsService;

    [ObservableProperty]
    private string selectedLanguage = "vi";

    [ObservableProperty]
    private Location? userLocation;

    [ObservableProperty]
    private bool isMapAvailable = AppConfig.HasConfiguredMapsKey();

    [ObservableProperty]
    private string mapStatus = "Map đang sẵn sàng.";

    public MapViewModel(PoiApiService poiApiService, LocationService locationService, SettingsService settingsService)
    {
        _poiApiService = poiApiService;
        _locationService = locationService;
        _settingsService = settingsService;

        Title = "Bản đồ ẩm thực";
        Pois = [];
    }

    public ObservableCollection<PoiResponse> Pois { get; }

    public async Task InitializeAsync()
    {
        SelectedLanguage = _settingsService.GetLanguage();
        await RefreshAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await RunBusyAsync(async () =>
        {
            var location = await _locationService.GetCurrentLocationAsync();
            UserLocation = location ?? new Location(AppConfig.DefaultLatitude, AppConfig.DefaultLongitude);
            var pois = await _poiApiService.LoadAllAsync(SelectedLanguage, null, null, null);
            Pois.ReplaceWith(pois.Where(static poi => poi.Latitude != 0 && poi.Longitude != 0).ToList());
            MapStatus = IsMapAvailable
                ? $"Đã tải {Pois.Count} POI trên bản đồ."
                : "Google Maps API key chưa được cấu hình. App đang dùng fallback danh sách POI.";
        }, "Không tải được dữ liệu bản đồ.");
    }

    [RelayCommand]
    private async Task OpenDetailAsync(string poiId) => await Shell.Current.GoToAsync($"poi-detail?id={poiId}");

    [RelayCommand]
    private async Task OpenExternalMapAsync(string poiId)
    {
        var poi = Pois.FirstOrDefault(item => item.Id == poiId);
        if (poi is null)
        {
            return;
        }

        await Launcher.Default.OpenAsync($"https://www.google.com/maps/search/?api=1&query={poi.Latitude},{poi.Longitude}");
    }

    [RelayCommand]
    private async Task CenterOnUserAsync()
    {
        if (UserLocation is null)
        {
            return;
        }

        await Launcher.Default.OpenAsync($"https://www.google.com/maps/search/?api=1&query={UserLocation.Latitude},{UserLocation.Longitude}");
    }
}
