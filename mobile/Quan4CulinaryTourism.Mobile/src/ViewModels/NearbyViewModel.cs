using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quan4CulinaryTourism.Mobile.Config;
using Quan4CulinaryTourism.Mobile.DTOs;
using Quan4CulinaryTourism.Mobile.Models;
using Quan4CulinaryTourism.Mobile.Services;
using System.Collections.ObjectModel;

namespace Quan4CulinaryTourism.Mobile.ViewModels;

public partial class NearbyViewModel : BaseViewModel
{
    private readonly PoiApiService _poiApiService;
    private readonly OfflinePackService _offlinePackService;
    private readonly ConnectivityService _connectivityService;
    private readonly LocationService _locationService;
    private readonly SettingsService _settingsService;
    private readonly AnalyticsApiService _analyticsApiService;

    [ObservableProperty]
    private int selectedRadius = 3000;

    [ObservableProperty]
    private string statusMessage = "Cho phep GPS de tim quan gan ban.";

    [ObservableProperty]
    private double latitude = AppConfig.DefaultLatitude;

    [ObservableProperty]
    private double longitude = AppConfig.DefaultLongitude;

    [ObservableProperty]
    private string selectedLanguage = "vi";

    [ObservableProperty]
    private bool hasNearbyResults;

    public NearbyViewModel(
        PoiApiService poiApiService,
        OfflinePackService offlinePackService,
        ConnectivityService connectivityService,
        LocationService locationService,
        SettingsService settingsService,
        AnalyticsApiService analyticsApiService)
    {
        _poiApiService = poiApiService;
        _offlinePackService = offlinePackService;
        _connectivityService = connectivityService;
        _locationService = locationService;
        _settingsService = settingsService;
        _analyticsApiService = analyticsApiService;

        Title = "Quan gan ban";
        NearbyPois = [];
        RadiusOptions = AppConfig.NearbyRadiusOptions;
    }

    public ObservableCollection<NearbyPoiResponse> NearbyPois { get; }
    public IReadOnlyList<int> RadiusOptions { get; }

    public async Task InitializeAsync()
    {
        SelectedLanguage = _settingsService.GetLanguage();
        await LoadNearbyAsync();
    }

    [RelayCommand]
    private async Task LoadNearbyAsync()
    {
        await RunBusyAsync(async () =>
        {
            var hasPermission = await _locationService.CheckAndRequestPermissionAsync();
            if (!hasPermission)
            {
                StatusMessage = "Chua co quyen GPS. App dang dung vi tri mac dinh Quan 4.";
            }

            var location = hasPermission ? await _locationService.GetCurrentLocationAsync() : null;
            Latitude = location?.Latitude ?? AppConfig.DefaultLatitude;
            Longitude = location?.Longitude ?? AppConfig.DefaultLongitude;

            List<NearbyPoiResponse> pois;
            if (_connectivityService.IsOnline())
            {
                pois = await _poiApiService.GetNearbyAsync(Latitude, Longitude, SelectedRadius, 20, SelectedLanguage);
                StatusMessage = $"Da tim {pois.Count} dia diem gan ban.";
            }
            else
            {
                pois = await _offlinePackService.GetNearbyOfflineAsync(Latitude, Longitude, SelectedRadius);
                StatusMessage = $"Dang offline. Tim thay {pois.Count} dia diem gan tu cache.";
            }

            NearbyPois.ReplaceWith(pois);
            HasNearbyResults = pois.Count > 0;

            await _analyticsApiService.CollectAsync(new CollectAnalyticsRequest
            {
                EventName = "nearby_requested",
                Lang = SelectedLanguage,
                Metadata =
                {
                    ["radius"] = SelectedRadius,
                    ["offline"] = !_connectivityService.IsOnline()
                }
            });
        }, "Khong lay duoc danh sach dia diem gan ban.");
    }

    [RelayCommand]
    private async Task OpenDetailAsync(string poiId) => await Shell.Current.GoToAsync($"poi-detail?id={poiId}");
}
