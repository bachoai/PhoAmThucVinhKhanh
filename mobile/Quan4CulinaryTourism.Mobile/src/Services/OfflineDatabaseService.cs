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

    public async Task ClearCacheAsync()
    {
        var db = await _dbContext.GetConnectionAsync();
        await db.DeleteAllAsync<LocalCategory>();
        await db.DeleteAllAsync<LocalPoi>();
        await db.DeleteAllAsync<LocalPoiDetail>();
        await db.DeleteAllAsync<LocalPoiAudio>();
        await db.DeleteAllAsync<LocalCacheMetadata>();
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
}
