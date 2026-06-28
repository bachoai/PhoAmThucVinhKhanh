using Microsoft.Maui.Devices.Sensors;
using Quan4CulinaryTourism.Mobile.DTOs;
using Quan4CulinaryTourism.Mobile.Models;

namespace Quan4CulinaryTourism.Mobile.Services;

public sealed class OfflinePackService
{
    private readonly CategoryApiService _categoryApiService;
    private readonly PoiApiService _poiApiService;
    private readonly AudioApiService _audioApiService;
    private readonly MapsApiService _mapsApiService;
    private readonly AudioDownloadService _audioDownloadService;
    private readonly OfflineDatabaseService _offlineDatabaseService;
    private readonly ConnectivityService _connectivityService;

    public OfflinePackService(
        CategoryApiService categoryApiService,
        PoiApiService poiApiService,
        AudioApiService audioApiService,
        MapsApiService mapsApiService,
        AudioDownloadService audioDownloadService,
        OfflineDatabaseService offlineDatabaseService,
        ConnectivityService connectivityService)
    {
        _categoryApiService = categoryApiService;
        _poiApiService = poiApiService;
        _audioApiService = audioApiService;
        _mapsApiService = mapsApiService;
        _audioDownloadService = audioDownloadService;
        _offlineDatabaseService = offlineDatabaseService;
        _connectivityService = connectivityService;
    }

    public async Task<OfflinePackStatus> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        await _offlineDatabaseService.InitializeAsync();
        var categoryCount = await _offlineDatabaseService.GetCategoryCountAsync();
        var poiCount = await _offlineDatabaseService.GetPoiCountAsync();
        var poiDetailCount = await _offlineDatabaseService.GetPoiDetailCountAsync();
        var offlineAudioCount = await _offlineDatabaseService.GetOfflineAudioCountAsync();
        var mapPack = await _offlineDatabaseService.GetMapPackAsync();
        var lastPreparedAtUtc = await _offlineDatabaseService.GetCacheUpdatedAtAsync("offline-pack");

        return new OfflinePackStatus
        {
            CategoryCount = categoryCount,
            PoiCount = poiCount,
            PoiDetailCount = poiDetailCount,
            OfflineAudioCount = offlineAudioCount,
            HasCategories = categoryCount > 0,
            HasPois = poiCount > 0,
            HasPoiDetails = poiDetailCount > 0,
            HasOfflineAudio = offlineAudioCount > 0,
            HasMapPack = mapPack is not null && HasUsableMapPack(mapPack),
            LastPreparedAtUtc = lastPreparedAtUtc
        };
    }

    public async Task<OfflinePackStatus> PrepareAsync(OfflinePackPrepareOptions options, CancellationToken cancellationToken = default)
    {
        if (!_connectivityService.IsOnline())
        {
            throw new InvalidOperationException("Can ket noi mang de chuan bi offline pack lan dau.");
        }

        await _offlineDatabaseService.InitializeAsync();

        var categories = await _categoryApiService.GetCategoriesAsync(cancellationToken);
        await _offlineDatabaseService.SaveCategoriesAsync(categories);

        var pois = await _poiApiService.LoadAllAsync(options.Language, null, null, null, cancellationToken);
        MapCategoryNames(categories, pois);
        await _offlineDatabaseService.SavePoisAsync(pois);

        foreach (var poi in pois)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var detail = await _poiApiService.GetByIdAsync(poi.Id, options.Language, cancellationToken);
            if (detail is not null)
            {
                detail.CategoryName = categories.FirstOrDefault(category => category.Id == detail.CategoryId)?.Name ?? "Khac";
                await _offlineDatabaseService.SavePoiDetailAsync(detail);
            }
        }

        await PrepareAudioAsync(pois, options, cancellationToken);
        await PrepareMapPackAsync(cancellationToken, options.DownloadMapPack);
        await _offlineDatabaseService.MarkCacheUpdatedAsync("offline-pack");

        return await GetStatusAsync(cancellationToken);
    }

    public async Task<List<NearbyPoiResponse>> GetNearbyOfflineAsync(double latitude, double longitude, int radiusMeters, CancellationToken cancellationToken = default)
    {
        await _offlineDatabaseService.InitializeAsync();
        var pois = await _offlineDatabaseService.GetPoisAsync();

        return pois
            .Where(static poi => poi.Latitude != 0 && poi.Longitude != 0)
            .Select(poi => new NearbyPoiResponse
            {
                Id = poi.Id,
                Name = poi.Name,
                Description = poi.Description,
                CategoryId = poi.CategoryId,
                Address = poi.Address,
                Ward = poi.Ward,
                District = poi.District,
                City = poi.City,
                PriceRange = poi.PriceRange,
                Rating = poi.Rating,
                ReviewCount = poi.ReviewCount,
                Priority = poi.Priority,
                MapUrl = poi.MapUrl,
                TtsScript = poi.TtsScript,
                Latitude = poi.Latitude,
                Longitude = poi.Longitude,
                Tags = poi.Tags,
                Images = poi.Images,
                IsActive = poi.IsActive,
                CategoryName = poi.CategoryName,
                HasAudio = poi.HasAudio,
                GeofenceRadiusMeters = poi.GeofenceRadiusMeters,
                AutoNarrationEnabled = poi.AutoNarrationEnabled,
                DistanceMeters = Location.CalculateDistance(latitude, longitude, poi.Latitude, poi.Longitude, DistanceUnits.Kilometers) * 1000d
            })
            .Where(poi => poi.DistanceMeters <= radiusMeters)
            .OrderBy(poi => poi.DistanceMeters)
            .Take(20)
            .ToList();
    }

    private async Task PrepareAudioAsync(List<PoiResponse> pois, OfflinePackPrepareOptions options, CancellationToken cancellationToken)
    {
        var audioManifest = await _audioApiService.GetPackManifestAsync(cancellationToken);
        if (audioManifest?.Items.Count > 0)
        {
            foreach (var audio in audioManifest.Items
                         .SelectMany(static item => item.Audios)
                         .Where(audio => string.Equals(audio.Lang, options.Language, StringComparison.OrdinalIgnoreCase)))
            {
                cancellationToken.ThrowIfCancellationRequested();
                await PrepareAudioEntryAsync(audio, options.DownloadAudio, cancellationToken);
            }

            return;
        }

        foreach (var poi in pois)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var audio = await _audioApiService.GetPoiAudioAsync(poi.Id, options.Language, cancellationToken);
            if (audio is null)
            {
                continue;
            }

            await PrepareAudioEntryAsync(audio, options.DownloadAudio, cancellationToken);
        }
    }

    private async Task PrepareAudioEntryAsync(PoiAudioResponse audio, bool downloadAudio, CancellationToken cancellationToken)
    {
        await _offlineDatabaseService.SavePoiAudioAsync(audio);
        if (downloadAudio && !string.IsNullOrWhiteSpace(audio.AudioUrl))
        {
            audio.LocalAudioPath = await _audioDownloadService.DownloadAsync(audio, cancellationToken);
            await _offlineDatabaseService.SavePoiAudioAsync(audio);
        }
    }

    private async Task PrepareMapPackAsync(CancellationToken cancellationToken, bool downloadMapPack)
    {
        var mapPack = await _mapsApiService.GetPackManifestAsync(cancellationToken);
        if (mapPack is null)
        {
            return;
        }

        if (downloadMapPack)
        {
            mapPack.LocalPackagePath = await _mapsApiService.DownloadPackAsync(mapPack, cancellationToken);
            mapPack.DownloadedAtUtc = string.IsNullOrWhiteSpace(mapPack.LocalPackagePath) ? null : DateTime.UtcNow;
        }

        await _offlineDatabaseService.SaveMapPackAsync(mapPack);
    }

    private static bool HasUsableMapPack(MapPackResponse mapPack)
    {
        if (!string.IsNullOrWhiteSpace(mapPack.LocalEntryHtmlPath) && File.Exists(mapPack.LocalEntryHtmlPath))
        {
            return true;
        }

        if (!string.IsNullOrWhiteSpace(mapPack.LocalPackagePath) && File.Exists(mapPack.LocalPackagePath))
        {
            return true;
        }

        return !string.IsNullOrWhiteSpace(mapPack.DownloadUrl);
    }

    private static void MapCategoryNames(IEnumerable<CategoryResponse> categories, IEnumerable<PoiResponse> pois)
    {
        var categoryMap = categories.ToDictionary(static category => category.Id, static category => category.Name);
        foreach (var poi in pois)
        {
            poi.CategoryName = categoryMap.TryGetValue(poi.CategoryId, out var name) ? name : "Khac";
        }
    }
}
