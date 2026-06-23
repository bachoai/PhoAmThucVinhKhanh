using MongoDB.Bson;
using MongoDB.Driver;
using Quan4CulinaryTourism.Api.Database;
using Quan4CulinaryTourism.Api.DTOs;
using Quan4CulinaryTourism.Api.Models;

namespace Quan4CulinaryTourism.Api.Repositories;

public class AnalyticsRepository
{
    private readonly MongoDbContext _context;
    public AnalyticsRepository(MongoDbContext context) => _context = context;

    public async Task CreateAsync(AnalyticsEvent entity, CancellationToken cancellationToken = default) =>
        await _context.AnalyticsEvents.InsertOneAsync(entity, cancellationToken: cancellationToken);

    public Task<long> CountByEventNameAsync(string eventName, CancellationToken cancellationToken = default) =>
        _context.AnalyticsEvents.CountDocumentsAsync(x => x.EventName == eventName, cancellationToken: cancellationToken);

    public Task<long> CountByEventNameAndPoiIdsAsync(string eventName, IEnumerable<string> poiIds, CancellationToken cancellationToken = default) =>
        _context.AnalyticsEvents.CountDocumentsAsync(x => x.EventName == eventName && poiIds.Contains(x.PoiId!), cancellationToken: cancellationToken);

    public async Task<List<TopPoiAnalyticsResponse>> GetTopPoiViewsAsync(CancellationToken cancellationToken = default) =>
        await GetTopByEventAsync("poi_viewed", cancellationToken);

    public async Task<List<TopPoiAnalyticsResponse>> GetTopAudioPlaysAsync(CancellationToken cancellationToken = default) =>
        await GetTopByEventAsync("audio_played", cancellationToken);

    private async Task<List<TopPoiAnalyticsResponse>> GetTopByEventAsync(string eventName, CancellationToken cancellationToken)
    {
        var results = await _context.AnalyticsEvents.Aggregate()
            .Match(x => x.EventName == eventName && x.PoiId != null)
            .Group(x => x.PoiId, x => new BsonDocument
            {
                { "_id", x.Key },
                { "count", x.Count() }
            })
            .Sort(new BsonDocument("count", -1))
            .Limit(10)
            .ToListAsync(cancellationToken);

        return results.Select(item => new TopPoiAnalyticsResponse
        {
            PoiId = item["_id"].AsString,
            Count = item["count"].ToInt64()
        }).ToList();
    }
}
