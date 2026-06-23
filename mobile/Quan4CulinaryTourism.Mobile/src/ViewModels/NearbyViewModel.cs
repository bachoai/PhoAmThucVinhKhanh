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
    private readonly LocationService _locationService;
    private readonly SettingsService _settingsService;
    private readonly AnalyticsApiService _analyticsApiService;

    [ObservableProperty]
    private int selectedRadius = 3000;

    [ObservableProperty]
    private string statusMessage = "Cho phép GPS để tìm quán gần bạn.";

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
        LocationService locationService,
        SettingsService settingsService,
        AnalyticsApiService analyticsApiService)
    {
        _poiApiService = poiApiService;
        _locationService = locationService;
        _settingsService = settingsService;
        _analyticsApiService = analyticsApiService;

        Title = "Quán gần bạn";
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
                StatusMessage = "Chưa có quyền GPS. App đang dùng vị trí mặc định Quận 4.";
            }

            var location = hasPermission ? await _locationService.GetCurrentLocationAsync() : null;
            Latitude = location?.Latitude ?? AppConfig.DefaultLatitude;
            Longitude = location?.Longitude ?? AppConfig.DefaultLongitude;

            var pois = await _poiApiService.GetNearbyAsync(Latitude, Longitude, SelectedRadius, 20, SelectedLanguage);
            NearbyPois.ReplaceWith(pois);
            HasNearbyResults = pois.Count > 0;
            StatusMessage = $"Đã tìm {pois.Count} địa điểm gần bạn.";

            await _analyticsApiService.CollectAsync(new CollectAnalyticsRequest
            {
                EventName = "nearby_requested",
                Lang = SelectedLanguage,
                Metadata = { ["radius"] = SelectedRadius }
            });
        }, "Không lấy được danh sách địa điểm gần bạn.");
    }

    [RelayCommand]
    private async Task OpenDetailAsync(string poiId) => await Shell.Current.GoToAsync($"poi-detail?id={poiId}");
}
