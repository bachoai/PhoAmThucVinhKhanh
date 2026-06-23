using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Devices.Sensors;
using Quan4CulinaryTourism.Mobile.DTOs;
using Quan4CulinaryTourism.Mobile.Models;

namespace Quan4CulinaryTourism.Mobile.Services;

public class GeofenceService
{
    private readonly PoiApiService _poiApiService;
    private readonly AudioApiService _audioApiService;
    private readonly AudioPlayerService _audioPlayerService;
    private readonly OfflineDatabaseService _offlineDatabaseService;
    private readonly LocationService _locationService;
    private readonly SettingsService _settingsService;
    private readonly AnalyticsApiService _analyticsApiService;
    private Task? _worker;
    private readonly CancellationTokenSource _cts = new();
    private readonly Dictionary<string, DateTime> _cooldownMap = [];

    public GeofenceService(
        PoiApiService poiApiService,
        AudioApiService audioApiService,
        AudioPlayerService audioPlayerService,
        OfflineDatabaseService offlineDatabaseService,
        LocationService locationService,
        SettingsService settingsService,
        AnalyticsApiService analyticsApiService)
    {
        _poiApiService = poiApiService;
        _audioApiService = audioApiService;
        _audioPlayerService = audioPlayerService;
        _offlineDatabaseService = offlineDatabaseService;
        _locationService = locationService;
        _settingsService = settingsService;
        _analyticsApiService = analyticsApiService;
    }

    public void EnsureStarted()
    {
        _worker ??= RunAsync(_cts.Token);
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(8));
        while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken))
        {
            if (!_settingsService.GetAutoNarrationEnabled() || _audioPlayerService.IsPlaying)
            {
                continue;
            }

            try
            {
                var hasPermission = await _locationService.CheckAndRequestPermissionAsync();
                if (!hasPermission)
                {
                    continue;
                }

                var location = await _locationService.GetCurrentLocationAsync();
                if (location is null)
                {
                    continue;
                }

                var pois = await _poiApiService.LoadAllAsync(_settingsService.GetLanguage(), null, null, null, cancellationToken);
                var defaultRadius = _settingsService.GetNarrationRadiusMeters();
                var candidate = pois
                    .Where(static poi => poi.Latitude != 0 && poi.Longitude != 0 && poi.AutoNarrationEnabled)
                    .Select(poi => new
                    {
                        Poi = poi,
                        Distance = Location.CalculateDistance(location, new Location(poi.Latitude, poi.Longitude), DistanceUnits.Kilometers) * 1000d,
                        Radius = poi.GeofenceRadiusMeters > 0 ? poi.GeofenceRadiusMeters : defaultRadius
                    })
                    .Where(item => item.Distance <= item.Radius)
                    .OrderByDescending(item => item.Poi.Priority)
                    .ThenBy(item => item.Distance)
                    .FirstOrDefault();

                if (candidate is null)
                {
                    continue;
                }

                if (_cooldownMap.TryGetValue(candidate.Poi.Id, out var lastTrigger) &&
                    DateTime.UtcNow - lastTrigger < TimeSpan.FromMinutes(5))
                {
                    continue;
                }

                _cooldownMap[candidate.Poi.Id] = DateTime.UtcNow;
                var language = _settingsService.GetLanguage();
                var audio = await _audioApiService.GetPoiAudioAsync(candidate.Poi.Id, language, cancellationToken)
                    ?? await _offlineDatabaseService.GetPoiAudioAsync(candidate.Poi.Id, language);

                if (!string.IsNullOrWhiteSpace(audio?.AudioUrl) || !string.IsNullOrWhiteSpace(audio?.LocalAudioPath))
                {
                    await _audioPlayerService.PlayAsync(audio!.AudioUrl, audio.LocalAudioPath);
                }
                else if (!string.IsNullOrWhiteSpace(candidate.Poi.Description))
                {
                    await TextToSpeech.Default.SpeakAsync(candidate.Poi.Description, cancelToken: cancellationToken);
                }

                await _analyticsApiService.CollectAsync(new CollectAnalyticsRequest
                {
                    EventName = "geofence_triggered",
                    PoiId = candidate.Poi.Id,
                    Lang = language,
                    Metadata =
                    {
                        ["distanceMeters"] = Math.Round(candidate.Distance),
                        ["radiusMeters"] = candidate.Radius
                    }
                }, cancellationToken);

                await Toast.Make($"Đang phát thuyết minh: {candidate.Poi.Name}").Show(cancellationToken);
            }
            catch
            {
                // Keep foreground geofence failures isolated.
            }
        }
    }
}
