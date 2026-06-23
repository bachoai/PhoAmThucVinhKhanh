using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Quan4CulinaryTourism.Mobile.DTOs;
using Quan4CulinaryTourism.Mobile.Models;
using Quan4CulinaryTourism.Mobile.Services;

namespace Quan4CulinaryTourism.Mobile.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly SettingsService _settingsService;
    private readonly OfflineDatabaseService _offlineDatabaseService;
    private readonly HealthApiService _healthApiService;
    private readonly AnalyticsApiService _analyticsApiService;

    [ObservableProperty]
    private LanguageOption? selectedLanguageOption;

    [ObservableProperty]
    private ThemeOption? selectedThemeOption;

    [ObservableProperty]
    private string backendStatus = "Đang kiểm tra...";

    [ObservableProperty]
    private string appVersion = string.Empty;

    [ObservableProperty]
    private bool autoNarrationEnabled;

    [ObservableProperty]
    private int selectedNarrationRadius = 150;

    [ObservableProperty]
    private string apiBaseUrl = string.Empty;

    [ObservableProperty]
    private ApiEndpointOption? selectedEndpointOption;

    public SettingsViewModel(
        SettingsService settingsService,
        OfflineDatabaseService offlineDatabaseService,
        HealthApiService healthApiService,
        AnalyticsApiService analyticsApiService)
    {
        _settingsService = settingsService;
        _offlineDatabaseService = offlineDatabaseService;
        _healthApiService = healthApiService;
        _analyticsApiService = analyticsApiService;

        Title = "Cài đặt";
        Languages = _settingsService.GetLanguages();
        Themes = _settingsService.GetThemes();
        ApiEndpoints = _settingsService.GetApiEndpointOptions();
    }

    public IReadOnlyList<LanguageOption> Languages { get; }
    public IReadOnlyList<ThemeOption> Themes { get; }
    public IReadOnlyList<ApiEndpointOption> ApiEndpoints { get; }
    public IReadOnlyList<int> NarrationRadiusOptions { get; } = [100, 150, 250, 500];

    public async Task InitializeAsync()
    {
        SelectedLanguageOption = Languages.FirstOrDefault(language => language.Code == _settingsService.GetLanguage()) ?? Languages.First();
        SelectedThemeOption = Themes.FirstOrDefault(theme => theme.Key == _settingsService.GetTheme()) ?? Themes.First();
        AutoNarrationEnabled = _settingsService.GetAutoNarrationEnabled();
        SelectedNarrationRadius = _settingsService.GetNarrationRadiusMeters();
        ApiBaseUrl = _settingsService.GetApiBaseUrl();
        SelectedEndpointOption = ApiEndpoints.FirstOrDefault(option => option.Url == ApiBaseUrl);
        AppVersion = AppInfo.Current.VersionString;

        var health = await _healthApiService.GetHealthAsync();
        BackendStatus = health is null
            ? "Backend chưa kết nối"
            : $"{health.Status} | Mongo: {(health.MongoConnected ? "OK" : "Fail")}";
    }

    [RelayCommand]
    private async Task ChangeLanguageAsync()
    {
        _settingsService.SetLanguage(SelectedLanguageOption?.Code ?? "vi");
        await _analyticsApiService.CollectAsync(new CollectAnalyticsRequest
        {
            EventName = "language_changed",
            Lang = SelectedLanguageOption?.Code ?? "vi"
        });
    }

    [RelayCommand]
    private Task ChangeThemeAsync()
    {
        _settingsService.SetTheme(SelectedThemeOption?.Key ?? "system");
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task SaveNarrationSettingsAsync()
    {
        _settingsService.SetAutoNarrationEnabled(AutoNarrationEnabled);
        _settingsService.SetNarrationRadiusMeters(SelectedNarrationRadius);
        BackendStatus = "Đã cập nhật Auto Narration.";
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task ApplyEndpointPresetAsync()
    {
        if (SelectedEndpointOption is null)
        {
            return Task.CompletedTask;
        }

        ApiBaseUrl = SelectedEndpointOption.Url;
        return SaveApiBaseUrlAsync();
    }

    [RelayCommand]
    private Task SaveApiBaseUrlAsync()
    {
        _settingsService.SetApiBaseUrl(ApiBaseUrl);
        BackendStatus = $"Đã cập nhật backend: {_settingsService.GetApiBaseUrl()}";
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task ClearCacheAsync()
    {
        await _offlineDatabaseService.ClearCacheAsync();
        BackendStatus = "Đã xóa cache offline.";
    }
}
