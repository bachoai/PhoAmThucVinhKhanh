using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quan4CulinaryTourism.Mobile.Models;
using Quan4CulinaryTourism.Mobile.Services;
using System.Collections.ObjectModel;

namespace Quan4CulinaryTourism.Mobile.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
    private readonly CategoryApiService _categoryApiService;
    private readonly PoiApiService _poiApiService;
    private readonly OfflineDatabaseService _offlineDatabaseService;
    private readonly ConnectivityService _connectivityService;
    private readonly SettingsService _settingsService;

    private List<PoiResponse> _allPois = [];
    private List<CategoryResponse> _allCategories = [];

    [ObservableProperty]
    private string selectedLanguage = "vi";

    [ObservableProperty]
    private string searchKeyword = string.Empty;

    [ObservableProperty]
    private bool isOfflineMode;

    [ObservableProperty]
    private string welcomeMessage = "Khám phá ẩm thực Quận 4";

    [ObservableProperty]
    private string heroSubtitle = "Đi qua các khu phố ẩm thực, nghe thuyết minh và khám phá điểm ăn uống nổi bật trong demo.";

    public HomeViewModel(
        CategoryApiService categoryApiService,
        PoiApiService poiApiService,
        OfflineDatabaseService offlineDatabaseService,
        ConnectivityService connectivityService,
        SettingsService settingsService)
    {
        _categoryApiService = categoryApiService;
        _poiApiService = poiApiService;
        _offlineDatabaseService = offlineDatabaseService;
        _connectivityService = connectivityService;
        _settingsService = settingsService;

        Title = "Trang chủ";
        Categories = [];
        FeaturedPois = [];
        NearbyPois = [];
    }

    public ObservableCollection<CategoryResponse> Categories { get; }
    public ObservableCollection<PoiResponse> FeaturedPois { get; }
    public ObservableCollection<PoiResponse> NearbyPois { get; }

    public async Task InitializeAsync()
    {
        SelectedLanguage = _settingsService.GetLanguage();
        await LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        await RunBusyAsync(async () =>
        {
            await _offlineDatabaseService.InitializeAsync();
            if (_connectivityService.IsOnline())
            {
                _allCategories = await _categoryApiService.GetCategoriesAsync();
                _allPois = await _poiApiService.LoadAllAsync(SelectedLanguage, null, null, null);

                MergeData();
                await _offlineDatabaseService.SaveCategoriesAsync(_allCategories);
                await _offlineDatabaseService.SavePoisAsync(_allPois);
                IsOfflineMode = false;
            }
            else
            {
                _allCategories = await _offlineDatabaseService.GetCategoriesAsync();
                _allPois = await _offlineDatabaseService.GetPoisAsync();
                MergeData();
                IsOfflineMode = true;
            }

            RefreshCollections();
        }, "Không tải được dữ liệu trang chủ.");
    }

    [RelayCommand]
    private async Task OpenPoiListAsync()
    {
        var route = string.IsNullOrWhiteSpace(SearchKeyword)
            ? "//poi-list"
            : $"//poi-list?keyword={Uri.EscapeDataString(SearchKeyword)}";
        await Shell.Current.GoToAsync(route);
    }

    [RelayCommand]
    private async Task OpenDetailAsync(string poiId) => await Shell.Current.GoToAsync($"poi-detail?id={poiId}");

    [RelayCommand]
    private async Task OpenNearbyAsync() => await Shell.Current.GoToAsync("//nearby");

    [RelayCommand]
    private async Task OpenMapAsync() => await Shell.Current.GoToAsync("//map");

    [RelayCommand]
    private async Task OpenSettingsAsync() => await Shell.Current.GoToAsync("//settings");

    [RelayCommand]
    private async Task OpenQrDemoAsync() => await Shell.Current.GoToAsync("qr-entry");

    private void MergeData()
    {
        var categoryMap = _allCategories.ToDictionary(static category => category.Id, static category => category.Name);
        foreach (var poi in _allPois)
        {
            poi.CategoryName = categoryMap.TryGetValue(poi.CategoryId, out var name) ? name : "Khác";
        }
    }

    private void RefreshCollections()
    {
        Categories.Clear();
        foreach (var category in _allCategories.OrderBy(static item => item.SortOrder))
        {
            Categories.Add(category);
        }

        FeaturedPois.Clear();
        foreach (var poi in _allPois.OrderByDescending(static item => item.Priority).ThenByDescending(static item => item.Rating).Take(5))
        {
            FeaturedPois.Add(poi);
        }

        NearbyPois.Clear();
        foreach (var poi in _allPois.OrderByDescending(static item => item.Rating).Take(5))
        {
            NearbyPois.Add(poi);
        }
    }
}
