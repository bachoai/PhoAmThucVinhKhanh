using System.Text.Json;
using Quan4CulinaryTourism.Mobile.LocalDatabase;
using Quan4CulinaryTourism.Mobile.Models;

namespace Quan4CulinaryTourism.Mobile.Services;

public class OfflineDatabaseService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly LocalDbContext _dbContext;

    public OfflineDatabaseService(LocalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InitializeAsync() => _ = await _dbContext.GetConnectionAsync();

    public async Task SaveCategoriesAsync(List<CategoryResponse> categories)
    {
        var db = await _dbContext.GetConnectionAsync();
        await db.DeleteAllAsync<LocalCategory>();
        var rows = categories.Select(category => new LocalCategory
        {
            Id = category.Id,
            JsonData = JsonSerializer.Serialize(category, JsonOptions)
        }).ToList();
        await db.InsertAllAsync(rows);
        await TouchAsync("categories");
    }

    public async Task<List<CategoryResponse>> GetCategoriesAsync()
    {
        var db = await _dbContext.GetConnectionAsync();
        var rows = await db.Table<LocalCategory>().ToListAsync();
        return rows.Select(static row => JsonSerializer.Deserialize<CategoryResponse>(row.JsonData, JsonOptions))
            .Where(static item => item is not null)
            .Cast<CategoryResponse>()
            .ToList();
    }

    public async Task SavePoisAsync(List<PoiResponse> pois)
    {
        var db = await _dbContext.GetConnectionAsync();
        await db.DeleteAllAsync<LocalPoi>();
        var rows = pois.Select(poi => new LocalPoi
        {
            Id = poi.Id,
            JsonData = JsonSerializer.Serialize(poi, JsonOptions)
        }).ToList();
        await db.InsertAllAsync(rows);
        await TouchAsync("pois");
    }

    public async Task<List<PoiResponse>> GetPoisAsync()
    {
        var db = await _dbContext.GetConnectionAsync();
        var rows = await db.Table<LocalPoi>().ToListAsync();
        return rows.Select(static row => JsonSerializer.Deserialize<PoiResponse>(row.JsonData, JsonOptions))
            .Where(static item => item is not null)
            .Cast<PoiResponse>()
            .ToList();
    }

    public async Task SavePoiDetailAsync(PoiDetailResponse detail)
    {
        var db = await _dbContext.GetConnectionAsync();
        await db.InsertOrReplaceAsync(new LocalPoiDetail
        {
            PoiId = detail.Id,
            JsonData = JsonSerializer.Serialize(detail, JsonOptions)
        });
        await TouchAsync($"poi-detail:{detail.Id}");
    }

    public async Task<PoiDetailResponse?> GetPoiDetailAsync(string poiId)
    {
        var db = await _dbContext.GetConnectionAsync();
        var row = await db.FindAsync<LocalPoiDetail>(poiId);
        return row is null ? null : JsonSerializer.Deserialize<PoiDetailResponse>(row.JsonData, JsonOptions);
    }

    public async Task SavePoiAudioAsync(PoiAudioResponse audio)
    {
        var db = await _dbContext.GetConnectionAsync();
        await db.InsertOrReplaceAsync(new LocalPoiAudio
        {
            CacheKey = $"{audio.PoiId}:{audio.Lang}",
            JsonData = JsonSerializer.Serialize(audio, JsonOptions)
        });
        await TouchAsync($"poi-audio:{audio.PoiId}:{audio.Lang}");
    }

    public async Task<PoiAudioResponse?> GetPoiAudioAsync(string poiId, string lang)
    {
        var db = await _dbContext.GetConnectionAsync();
        var row = await db.FindAsync<LocalPoiAudio>($"{poiId}:{lang}");
        return row is null ? null : JsonSerializer.Deserialize<PoiAudioResponse>(row.JsonData, JsonOptions);
    }

    public async Task SaveMapPackAsync(MapPackResponse mapPack)
    {
        var db = await _dbContext.GetConnectionAsync();
        await db.InsertOrReplaceAsync(new LocalMapPack
        {
            CacheKey = "active",
            JsonData = JsonSerializer.Serialize(mapPack, JsonOptions)
        });
        await TouchAsync("map-pack");
    }

    public async Task<MapPackResponse?> GetMapPackAsync()
    {
        var db = await _dbContext.GetConnectionAsync();
        var row = await db.FindAsync<LocalMapPack>("active");
        return row is null ? null : JsonSerializer.Deserialize<MapPackResponse>(row.JsonData, JsonOptions);
    }

    public async Task<int> GetCategoryCountAsync()
    {
        var db = await _dbContext.GetConnectionAsync();
        return await db.Table<LocalCategory>().CountAsync();
    }

    public async Task<int> GetPoiCountAsync()
    {
        var db = await _dbContext.GetConnectionAsync();
        return await db.Table<LocalPoi>().CountAsync();
    }

    public async Task<int> GetPoiDetailCountAsync()
    {
        var db = await _dbContext.GetConnectionAsync();
        return await db.Table<LocalPoiDetail>().CountAsync();
    }

    public async Task<int> GetOfflineAudioCountAsync()
    {
        var db = await _dbContext.GetConnectionAsync();
        var rows = await db.Table<LocalPoiAudio>().ToListAsync();
        return rows
            .Select(static row => JsonSerializer.Deserialize<PoiAudioResponse>(row.JsonData, JsonOptions))
            .Count(static audio => !string.IsNullOrWhiteSpace(audio?.LocalAudioPath) && File.Exists(audio.LocalAudioPath));
    }

    public async Task<DateTime?> GetCacheUpdatedAtAsync(string key)
    {
        var db = await _dbContext.GetConnectionAsync();
        var row = await db.FindAsync<LocalCacheMetadata>(key);
        return row?.UpdatedAtUtc;
    }

    public Task MarkCacheUpdatedAsync(string key) => TouchAsync(key);

    public async Task ClearCacheAsync()
    {
        var db = await _dbContext.GetConnectionAsync();
        await db.DeleteAllAsync<LocalCategory>();
        await db.DeleteAllAsync<LocalPoi>();
        await db.DeleteAllAsync<LocalPoiDetail>();
        await db.DeleteAllAsync<LocalPoiAudio>();
        await db.DeleteAllAsync<LocalMapPack>();
        await db.DeleteAllAsync<LocalCacheMetadata>();

        DeleteFolderIfExists(Path.Combine(FileSystem.AppDataDirectory, "audio"));
        DeleteFolderIfExists(Path.Combine(FileSystem.AppDataDirectory, "maps"));
    }

    private async Task TouchAsync(string key)
    {
        var db = await _dbContext.GetConnectionAsync();
        await db.InsertOrReplaceAsync(new LocalCacheMetadata
        {
            CacheKey = key,
            UpdatedAtUtc = DateTime.UtcNow
        });
    }

    private static void DeleteFolderIfExists(string path)
    {
        if (!Directory.Exists(path))
        {
            return;
        }

        Directory.Delete(path, true);
    }
}
