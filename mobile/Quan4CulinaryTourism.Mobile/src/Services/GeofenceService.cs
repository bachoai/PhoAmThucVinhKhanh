using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Devices.Sensors;
using Quan4CulinaryTourism.Mobile.DTOs;
using Quan4CulinaryTourism.Mobile.Models;

namespace Quan4CulinaryTourism.Mobile.Services;

public class GeofenceService
{
    private static readonly TimeSpan PoiCacheDuration = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan LocationAnalyticsInterval = TimeSpan.FromSeconds(20);
    private const double MinLocationAnalyticsDistanceMeters = 25;

    private readonly PoiApiService _poiApiService;
    private readonly OfflineDatabaseService _offlineDatabaseService;
    private readonly LocationTrackingService _locationTrackingService;
    private readonly SettingsService _settingsService;
    private readonly AnalyticsApiService _analyticsApiService;
    private readonly NarrationEngineService _narrationEngineService;
    private readonly SemaphoreSlim _processingLock = new(1, 1);
    private List<PoiResponse> _cachedPois = [];
    private DateTime _cachedPoisAtUtc = DateTime.MinValue;
    private string _cachedLanguage = string.Empty;
    private bool _started;
    private LocationTrackingSample? _lastTrackedAnalyticsSample;
    private DateTimeOffset _lastTrackedAnalyticsAtUtc = DateTimeOffset.MinValue;

    public GeofenceService(
        PoiApiService poiApiService,
        OfflineDatabaseService offlineDatabaseService,
        LocationTrackingService locationTrackingService,
        SettingsService settingsService,
        AnalyticsApiService analyticsApiService,
        NarrationEngineService narrationEngineService)
    {
        _poiApiService = poiApiService;
        _offlineDatabaseService = offlineDatabaseService;
        _locationTrackingService = locationTrackingService;
        _settingsService = settingsService;
        _analyticsApiService = analyticsApiService;
        _narrationEngineService = narrationEngineService;
    }

    public void EnsureStarted()
    {
        if (_started)
        {
            return;
        }

        _started = true;
        _locationTrackingService.LocationChanged += OnLocationChanged;
        _locationTrackingService.EnsureStarted();
        _ = PrimeAsync();
    }

    public async Task PrimeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await GetTrackedPoisAsync(_settingsService.GetLanguage(), forceRefresh: true, cancellationToken);
        }
        catch
        {
            // Keep POI preload failures from blocking app startup.
        }
    }

    private void OnLocationChanged(object? sender, LocationTrackingSample sample)
    {
        _ = HandleLocationAsync(sample);
    }

    private async Task HandleLocationAsync(LocationTrackingSample sample)
    {
        if (!_settingsService.GetAutoNarrationEnabled())
        {
            return;
        }

        if (!await _processingLock.WaitAsync(0))
        {
            return;
        }

        try
        {
            var language = _settingsService.GetLanguage();
            await TrackLocationSampleAsync(sample, language);
            var pois = await GetTrackedPoisAsync(language);
            var defaultRadius = _settingsService.GetNarrationRadiusMeters();
            var candidate = pois
                .Where(static poi => poi.Latitude != 0 && poi.Longitude != 0 && poi.AutoNarrationEnabled)
                .Select(poi => new
                {
                    Poi = poi,
                    Distance = Location.CalculateDistance(sample.Location, new Location(poi.Latitude, poi.Longitude), DistanceUnits.Kilometers) * 1000d,
                    Radius = poi.GeofenceRadiusMeters > 0 ? poi.GeofenceRadiusMeters : defaultRadius
                })
                .Where(item => item.Distance <= item.Radius)
                .OrderByDescending(item => item.Poi.Priority)
                .ThenBy(item => item.Distance)
                .FirstOrDefault();

            if (candidate is null)
            {
                return;
            }

            var queued = await _narrationEngineService.TryQueueAutoNarrationAsync(
                candidate.Poi,
                language,
                sample,
                candidate.Distance,
                candidate.Radius);
            if (!queued)
            {
                return;
            }

            if (!sample.IsBackground)
            {
                await Toast.Make($"Da them thuyet minh vao hang cho: {candidate.Poi.Name}").Show();
            }
        }
        catch
        {
            // Keep location-triggered playback failures isolated.
        }
        finally
        {
            _processingLock.Release();
        }
    }

    private async Task<List<PoiResponse>> GetTrackedPoisAsync(string language, bool forceRefresh = false, CancellationToken cancellationToken = default)
    {
        var shouldRefresh = forceRefresh
            || _cachedPois.Count == 0
            || !string.Equals(_cachedLanguage, language, StringComparison.Ordinal)
            || DateTime.UtcNow - _cachedPoisAtUtc > PoiCacheDuration;

        if (!shouldRefresh)
        {
            return _cachedPois;
        }

        var pois = await _poiApiService.LoadAllAsync(language, null, null, null, cancellationToken);
        if (pois.Count > 0)
        {
            _cachedPois = pois;
            _cachedLanguage = language;
            _cachedPoisAtUtc = DateTime.UtcNow;
            await _offlineDatabaseService.SavePoisAsync(pois);
            return _cachedPois;
        }

        var offlinePois = await _offlineDatabaseService.GetPoisAsync();
        if (offlinePois.Count > 0)
        {
            _cachedPois = offlinePois;
            _cachedLanguage = language;
            _cachedPoisAtUtc = DateTime.UtcNow;
        }

        return _cachedPois;
    }

    private async Task TrackLocationSampleAsync(LocationTrackingSample sample, string language)
    {
        if (!ShouldTrackLocationSample(sample))
        {
            return;
        }

        _lastTrackedAnalyticsSample = sample;
        _lastTrackedAnalyticsAtUtc = sample.TimestampUtc;

        await _analyticsApiService.CollectAsync(new CollectAnalyticsRequest
        {
            EventName = "location_sample",
            Lang = language,
            Metadata =
            {
                ["latitude"] = Math.Round(sample.Location.Latitude, 6),
                ["longitude"] = Math.Round(sample.Location.Longitude, 6),
                ["accuracyMeters"] = sample.AccuracyMeters ?? sample.Location.Accuracy ?? 0d,
                ["background"] = sample.IsBackground,
                ["trackingSource"] = sample.Source,
                ["speedMetersPerSecond"] = sample.Location.Speed ?? 0d,
                ["course"] = sample.Location.Course ?? 0d
            }
        });
    }

    private bool ShouldTrackLocationSample(LocationTrackingSample sample)
    {
        if (_lastTrackedAnalyticsSample is null)
        {
            return true;
        }

        if (sample.TimestampUtc - _lastTrackedAnalyticsAtUtc >= LocationAnalyticsInterval)
        {
            return true;
        }

        var distanceMeters = Location.CalculateDistance(sample.Location, _lastTrackedAnalyticsSample.Location, DistanceUnits.Kilometers) * 1000d;
        return distanceMeters >= MinLocationAnalyticsDistanceMeters;
    }
}
