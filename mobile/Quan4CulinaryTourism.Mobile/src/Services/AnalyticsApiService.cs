using System.Diagnostics;
using Microsoft.Maui.ApplicationModel;
using Quan4CulinaryTourism.Mobile.DTOs;

namespace Quan4CulinaryTourism.Mobile.Services;

public class AnalyticsApiService
{
    private readonly ApiClient _apiClient;
    private readonly SettingsService _settingsService;

    public AnalyticsApiService(ApiClient apiClient, SettingsService settingsService)
    {
        _apiClient = apiClient;
        _settingsService = settingsService;
    }

    public async Task CollectAsync(CollectAnalyticsRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            request.AnonymousId ??= _settingsService.GetOrCreateAnonymousId();
            request.SessionId ??= _settingsService.GetOrCreateSessionId();
            request.PageViewId ??= Guid.NewGuid().ToString("N");
            request.Metadata["source"] = "mobile";
            request.Metadata["platform"] = DeviceInfo.Current.Platform.ToString();
            request.Metadata["appVersion"] = AppInfo.Current.VersionString;

            await _apiClient.PostAsync<CollectAnalyticsRequest, object?>("/api/v1/analytics/collect", request, cancellationToken);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Analytics failed: {ex.Message}");
        }
    }
}
