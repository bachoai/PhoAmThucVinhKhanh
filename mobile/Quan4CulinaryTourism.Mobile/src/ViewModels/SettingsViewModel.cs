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
    private readonly OfflinePackService _offlinePackService;
    private readonly ConnectivityService _connectivityService;
    private readonly HealthApiService _healthApiService;
    private readonly AnalyticsApiService _analyticsApiService;
    private readonly LocationTrackingService _locationTrackingService;

    [ObservableProperty]
    private LanguageOption? selectedLanguageOption;

    [ObservableProperty]
    private ThemeOption? selectedThemeOption;

    [ObservableProperty]
    private string backendStatus = "Dang kiem tra...";

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

    [ObservableProperty]
    private string trackingStatus = string.Empty;

    [ObservableProperty]
    private string offlinePackStatus = "Dang kiem tra offline pack...";

    [ObservableProperty]
    private string offlinePackSummary = string.Empty;

    [ObservableProperty]
    private bool downloadAudioForOffline = true;

    [ObservableProperty]
    private bool downloadMapPackForOffline;

    [ObservableProperty]
    private bool offlinePackReady;

    public SettingsViewModel(
        SettingsService settingsService,
        OfflineDatabaseService offlineDatabaseService,
        OfflinePackService offlinePackService,
        ConnectivityService connectivityService,
        HealthApiService healthApiService,
        AnalyticsApiService analyticsApiService,
        LocationTrackingService locationTrackingService)
    {
        _settingsService = settingsService;
        _offlineDatabaseService = offlineDatabaseService;
        _offlinePackService = offlinePackService;
        _connectivityService = connectivityService;
        _healthApiService = healthApiService;
        _analyticsApiService = analyticsApiService;
        _locationTrackingService = locationTrackingService;

        Title = "Cai dat";
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
        TrackingStatus = _locationTrackingService.GetStatusText();
        await RefreshOfflineStatusAsync();

        var health = await _healthApiService.GetHealthAsync();
        BackendStatus = health is null
            ? "Backend chua ket noi"
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
    private async Task SaveNarrationSettingsAsync()
    {
        _settingsService.UpdateAutoNarrationSettings(AutoNarrationEnabled, SelectedNarrationRadius);

        if (AutoNarrationEnabled)
        {
            await _locationTrackingService.EnsureStartedAsync();
        }
        else
        {
            await _locationTrackingService.StopAsync();
        }

        TrackingStatus = _locationTrackingService.GetStatusText();
        BackendStatus = "Da cap nhat Auto Narration va GPS tracking.";
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
        BackendStatus = $"Da cap nhat backend: {_settingsService.GetApiBaseUrl()}";
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task PrepareOfflinePackAsync()
    {
        await RunBusyAsync(async () =>
        {
            var status = await _offlinePackService.PrepareAsync(new OfflinePackPrepareOptions
            {
                Language = SelectedLanguageOption?.Code ?? _settingsService.GetLanguage(),
                DownloadAudio = DownloadAudioForOffline,
                DownloadMapPack = DownloadMapPackForOffline
            });

            ApplyOfflineStatus(status);
            BackendStatus = _connectivityService.IsOnline()
                ? "Da chuan bi offline pack thanh cong."
                : "Da cap nhat thong tin offline tu cache.";
        }, "Khong chuan bi duoc offline pack.");
    }

    [RelayCommand]
    private async Task RefreshOfflineStatusAsync()
    {
        var status = await _offlinePackService.GetStatusAsync();
        ApplyOfflineStatus(status);
    }

    [RelayCommand]
    private async Task ClearCacheAsync()
    {
        await _offlineDatabaseService.ClearCacheAsync();
        BackendStatus = "Da xoa cache offline.";
        await RefreshOfflineStatusAsync();
    }

    private void ApplyOfflineStatus(OfflinePackStatus status)
    {
        OfflinePackReady = status.IsReady;
        OfflinePackSummary = status.Summary;
        OfflinePackStatus = status.LastPreparedAtUtc is null
            ? "Offline pack chua duoc chuan bi."
            : $"Lan sync cuoi: {status.LastPreparedAtUtc.Value.ToLocalTime():dd/MM HH:mm}";
    }
}
