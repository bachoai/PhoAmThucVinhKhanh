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
    private readonly OfflineDatabaseService _offlineDatabaseService;
    private readonly ConnectivityService _connectivityService;
    private readonly MapsApiService _mapsApiService;
    private readonly OfflineMapService _offlineMapService;
    private readonly LocationService _locationService;
    private readonly SettingsService _settingsService;

    [ObservableProperty]
    private string selectedLanguage = "vi";

    [ObservableProperty]
    private Location? userLocation;

    [ObservableProperty]
    private bool isMapAvailable = AppConfig.HasConfiguredMapsKey();

    [ObservableProperty]
    private bool useOfflineMap;

    [ObservableProperty]
    private string? offlineMapHtmlPath;

    [ObservableProperty]
    private string mapStatus = "Map dang san sang.";

    public MapViewModel(
        PoiApiService poiApiService,
        OfflineDatabaseService offlineDatabaseService,
        ConnectivityService connectivityService,
        MapsApiService mapsApiService,
        OfflineMapService offlineMapService,
        LocationService locationService,
        SettingsService settingsService)
    {
        _poiApiService = poiApiService;
        _offlineDatabaseService = offlineDatabaseService;
        _connectivityService = connectivityService;
        _mapsApiService = mapsApiService;
        _offlineMapService = offlineMapService;
        _locationService = locationService;
        _settingsService = settingsService;

        Title = "Ban do am thuc";
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
            await _offlineDatabaseService.InitializeAsync();
            var location = await _locationService.GetCurrentLocationAsync();
            UserLocation = location ?? new Location(AppConfig.DefaultLatitude, AppConfig.DefaultLongitude);

            List<PoiResponse> pois;
            var mapPack = await _offlineDatabaseService.GetMapPackAsync();
            var isOnline = _connectivityService.IsOnline();

            if (isOnline)
            {
                pois = await _poiApiService.LoadAllAsync(SelectedLanguage, null, null, null);
                if (pois.Count > 0)
                {
                    await _offlineDatabaseService.SavePoisAsync(pois);
                }

                var liveMapPack = await _mapsApiService.GetPackManifestAsync();
                if (liveMapPack is not null)
                {
                    if (mapPack is not null && string.Equals(mapPack.Version, liveMapPack.Version, StringComparison.OrdinalIgnoreCase))
                    {
                        liveMapPack.LocalPackagePath = mapPack.LocalPackagePath;
                        liveMapPack.ExtractedDirectoryPath = mapPack.ExtractedDirectoryPath;
                        liveMapPack.LocalEntryHtmlPath = mapPack.LocalEntryHtmlPath;
                        liveMapPack.DownloadedAtUtc = mapPack.DownloadedAtUtc;
                    }

                    await _offlineDatabaseService.SaveMapPackAsync(liveMapPack);
                    mapPack = liveMapPack;
                }
            }
            else
            {
                pois = await _offlineDatabaseService.GetPoisAsync();
            }

            var validPois = pois.Where(static poi => poi.Latitude != 0 && poi.Longitude != 0).ToList();
            Pois.ReplaceWith(validPois);

            mapPack = await _offlineMapService.PrepareRenderablePackAsync(mapPack, validPois, UserLocation);
            OfflineMapHtmlPath = mapPack?.LocalEntryHtmlPath;
            UseOfflineMap = !string.IsNullOrWhiteSpace(OfflineMapHtmlPath) && (!isOnline || !IsMapAvailable);

            MapStatus = BuildMapStatus(mapPack, isOnline, validPois.Count);
        }, "Khong tai duoc du lieu ban do.");
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

    private string BuildMapStatus(MapPackResponse? mapPack, bool isOnline, int poiCount)
    {
        if (UseOfflineMap)
        {
            var packName = mapPack is null ? "runtime" : $"{mapPack.Name} {mapPack.Version}".Trim();
            return isOnline
                ? $"Dang dung offline map engine {packName} voi {poiCount} POI."
                : $"Dang offline. Ban do local {packName} dang hien {poiCount} POI.";
        }

        if (IsMapAvailable)
        {
            return isOnline
                ? $"Da tai {poiCount} POI tren native map."
                : $"Dang mo danh sach va du lieu cache voi {poiCount} POI.";
        }

        if (mapPack is not null && !string.IsNullOrWhiteSpace(mapPack.DownloadUrl))
        {
            return $"Map key chua cau hinh. App co san offline pack {mapPack.Name} {mapPack.Version}.";
        }

        return isOnline
            ? "Map key chua cau hinh. App dang dung danh sach POI."
            : "Dang offline. App dang dung danh sach POI cache thay cho map.";
    }
}
