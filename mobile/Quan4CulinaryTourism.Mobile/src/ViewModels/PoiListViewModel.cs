using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quan4CulinaryTourism.Mobile.DTOs;
using Quan4CulinaryTourism.Mobile.Models;
using Quan4CulinaryTourism.Mobile.Services;
using System.Collections.ObjectModel;

namespace Quan4CulinaryTourism.Mobile.ViewModels;

public partial class PoiListViewModel : BaseViewModel
{
    private readonly PoiApiService _poiApiService;
    private readonly CategoryApiService _categoryApiService;
    private readonly OfflineDatabaseService _offlineDatabaseService;
    private readonly ConnectivityService _connectivityService;
    private readonly SettingsService _settingsService;
    private readonly AnalyticsApiService _analyticsApiService;

    private List<PoiResponse> _allPois = [];

    [ObservableProperty]
    private string selectedLanguage = "vi";

    [ObservableProperty]
    private string searchKeyword = string.Empty;

    [ObservableProperty]
    private CategoryResponse? selectedCategory;

    [ObservableProperty]
    private string selectedPriceRange = string.Empty;

    [ObservableProperty]
    private bool isOfflineMode;

    [ObservableProperty]
    private string emptyStateMessage = "Chưa có địa điểm phù hợp với bộ lọc hiện tại.";

    public PoiListViewModel(
        PoiApiService poiApiService,
        CategoryApiService categoryApiService,
        OfflineDatabaseService offlineDatabaseService,
        ConnectivityService connectivityService,
        SettingsService settingsService,
        AnalyticsApiService analyticsApiService)
    {
        _poiApiService = poiApiService;
        _categoryApiService = categoryApiService;
        _offlineDatabaseService = offlineDatabaseService;
        _connectivityService = connectivityService;
        _settingsService = settingsService;
        _analyticsApiService = analyticsApiService;

        Title = "Địa điểm ẩm thực";
        Categories = [];
        Pois = [];
        PriceRanges = ["", "$", "$$", "$$$"];
    }

    public ObservableCollection<CategoryResponse> Categories { get; }
    public ObservableCollection<PoiResponse> Pois { get; }
    public IReadOnlyList<string> PriceRanges { get; }

    public async Task InitializeAsync(string? keyword = null)
    {
        SelectedLanguage = _settingsService.GetLanguage();
        SearchKeyword = keyword ?? SearchKeyword;
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
                var categories = await _categoryApiService.GetCategoriesAsync();
                _allPois = await _poiApiService.LoadAllAsync(SelectedLanguage, SearchKeyword, SelectedCategory?.Id, SelectedPriceRange);
                MapCategoryNames(categories, _allPois);
                await _offlineDatabaseService.SaveCategoriesAsync(categories);
                await _offlineDatabaseService.SavePoisAsync(_allPois);
                Categories.ReplaceWith(categories);
                IsOfflineMode = false;
            }
            else
            {
                var categories = await _offlineDatabaseService.GetCategoriesAsync();
                _allPois = await _offlineDatabaseService.GetPoisAsync();
                MapCategoryNames(categories, _allPois);
                Categories.ReplaceWith(categories);
                IsOfflineMode = true;
            }

            ApplyFilters();
        }, "Không tải được danh sách địa điểm.");
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadAsync();
        IsRefreshing = false;
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await _analyticsApiService.CollectAsync(new CollectAnalyticsRequest
        {
            EventName = "search_executed",
            Lang = SelectedLanguage,
            Metadata =
            {
                ["categoryId"] = SelectedCategory?.Id ?? string.Empty,
                ["priceRange"] = SelectedPriceRange
            }
        });

        await LoadAsync();
    }

    [RelayCommand]
    private void Filter() => ApplyFilters();

    [RelayCommand]
    private async Task OpenDetailAsync(string poiId) => await Shell.Current.GoToAsync($"poi-detail?id={poiId}");

    private void ApplyFilters()
    {
        var query = _allPois.AsEnumerable();

        if (!_connectivityService.IsOnline())
        {
            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                query = query.Where(poi => poi.Name.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase)
                    || poi.Description.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(SelectedCategory?.Id))
            {
                query = query.Where(poi => poi.CategoryId == SelectedCategory!.Id);
            }

            if (!string.IsNullOrWhiteSpace(SelectedPriceRange))
            {
                query = query.Where(poi => poi.PriceRange == SelectedPriceRange);
            }
        }

        var filtered = query.ToList();
        EmptyStateMessage = filtered.Count == 0
            ? "Chưa có địa điểm phù hợp với bộ lọc hiện tại."
            : $"Đang hiển thị {filtered.Count} địa điểm.";
        Pois.ReplaceWith(filtered);
    }

    private static void MapCategoryNames(IEnumerable<CategoryResponse> categories, IEnumerable<PoiResponse> pois)
    {
        var categoryMap = categories.ToDictionary(static category => category.Id, static category => category.Name);
        foreach (var poi in pois)
        {
            poi.CategoryName = categoryMap.TryGetValue(poi.CategoryId, out var name) ? name : "Khác";
        }
    }
}
