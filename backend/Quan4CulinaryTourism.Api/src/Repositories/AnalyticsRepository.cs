using MongoDB.Bson;
using MongoDB.Driver;
using Quan4CulinaryTourism.Api.Common;
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

    public Task<long> CountByEventNamesAsync(IEnumerable<string> eventNames, CancellationToken cancellationToken = default)
    {
        var names = eventNames.Distinct(StringComparer.Ordinal).ToList();
        return _context.AnalyticsEvents.CountDocumentsAsync(x => names.Contains(x.EventName), cancellationToken: cancellationToken);
    }

    public Task<long> CountByEventNameAndPoiIdsAsync(string eventName, IEnumerable<string> poiIds, CancellationToken cancellationToken = default) =>
        _context.AnalyticsEvents.CountDocumentsAsync(x => x.EventName == eventName && poiIds.Contains(x.PoiId!), cancellationToken: cancellationToken);

    public Task<long> CountByEventNamesAndPoiIdsAsync(IEnumerable<string> eventNames, IEnumerable<string> poiIds, CancellationToken cancellationToken = default)
    {
        var names = eventNames.Distinct(StringComparer.Ordinal).ToList();
        var poiIdList = poiIds.Distinct(StringComparer.Ordinal).ToList();
        return _context.AnalyticsEvents.CountDocumentsAsync(
            x => names.Contains(x.EventName) && x.PoiId != null && poiIdList.Contains(x.PoiId),
            cancellationToken: cancellationToken);
    }

    public async Task<List<TopPoiAnalyticsResponse>> GetTopPoiViewsAsync(CancellationToken cancellationToken = default) =>
        await GetTopByEventAsync("poi_viewed", cancellationToken);

    public async Task<List<TopPoiAnalyticsResponse>> GetTopAudioPlaysAsync(CancellationToken cancellationToken = default) =>
        await GetTopByEventsAsync(["audio_played", "tts_played"], cancellationToken);

    public async Task<double> GetAverageListenDurationSecondsAsync(CancellationToken cancellationToken = default)
    {
        var items = await _context.AnalyticsEvents.Find(x => x.ListenDurationSeconds != null && x.ListenDurationSeconds > 0)
            .Limit(2000)
            .ToListAsync(cancellationToken);

        return items.Count == 0
            ? 0
            : Math.Round(items.Average(x => x.ListenDurationSeconds ?? 0), 1);
    }

    public async Task<List<AnalyticsHeatmapPointResponse>> GetHeatmapPointsAsync(int maxPoints = 80, CancellationToken cancellationToken = default)
    {
        var items = await _context.AnalyticsEvents.Find(x => x.EventName == "location_sample" && x.Latitude != null && x.Longitude != null)
            .SortByDescending(x => x.CreatedAt)
            .Limit(2000)
            .ToListAsync(cancellationToken);

        return items
            .GroupBy(
                item => new
                {
                    Latitude = Math.Round(item.Latitude ?? 0, 4),
                    Longitude = Math.Round(item.Longitude ?? 0, 4)
                })
            .Select(group => new AnalyticsHeatmapPointResponse
            {
                Latitude = group.Key.Latitude,
                Longitude = group.Key.Longitude,
                Count = group.LongCount(),
                LastSeenAt = group.Max(x => x.CreatedAt)
            })
            .OrderByDescending(x => x.Count)
            .ThenByDescending(x => x.LastSeenAt)
            .Take(maxPoints)
            .ToList();
    }

    public async Task<List<AnalyticsRouteTraceResponse>> GetRecentRouteTracesAsync(int maxRoutes = 6, int maxPointsPerRoute = 25, CancellationToken cancellationToken = default)
    {
        var items = await _context.AnalyticsEvents.Find(x =>
                x.EventName == "location_sample"
                && x.Latitude != null
                && x.Longitude != null
                && x.AnonymousId != null)
            .SortByDescending(x => x.CreatedAt)
            .Limit(1500)
            .ToListAsync(cancellationToken);

        return items
            .GroupBy(x => $"{x.AnonymousId}:{x.SessionId}")
            .Select(group =>
            {
                var orderedPoints = group.OrderBy(x => x.CreatedAt).ToList();
                var trimmedPoints = orderedPoints.Count > maxPointsPerRoute
                    ? orderedPoints.Skip(orderedPoints.Count - maxPointsPerRoute).ToList()
                    : orderedPoints;

                return new AnalyticsRouteTraceResponse
                {
                    AnonymousId = group.First().AnonymousId ?? string.Empty,
                    SessionId = group.First().SessionId,
                    PointCount = orderedPoints.Count,
                    StartedAt = orderedPoints.First().CreatedAt,
                    EndedAt = orderedPoints.Last().CreatedAt,
                    Points = trimmedPoints.Select(point => new AnalyticsRoutePointResponse
                    {
                        Latitude = point.Latitude ?? 0,
                        Longitude = point.Longitude ?? 0,
                        IsBackground = point.IsBackground ?? false,
                        Source = point.TrackingSource ?? "unknown",
                        CreatedAt = point.CreatedAt
                    }).ToList()
                };
            })
            .Where(trace => trace.PointCount > 1)
            .OrderByDescending(trace => trace.EndedAt)
            .Take(maxRoutes)
            .ToList();
    }

    public async Task<PagedResponse<UsageHistoryEntryResponse>> SearchUsageHistoryAsync(UsageHistoryRequest request, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 10, 200);
        var filter = BuildUsageHistoryFilter(request);

        var totalItems = await _context.AnalyticsEvents.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        var items = await _context.AnalyticsEvents.Find(filter)
            .SortByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<UsageHistoryEntryResponse>
        {
            Items = items.Select(item => new UsageHistoryEntryResponse
            {
                Id = item.Id,
                AnonymousId = item.AnonymousId,
                SessionId = item.SessionId,
                PageViewId = item.PageViewId,
                EventName = item.EventName,
                PoiId = item.PoiId,
                Lang = item.Lang,
                Metadata = item.Metadata,
                CreatedAt = item.CreatedAt
            }).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }

    private async Task<List<TopPoiAnalyticsResponse>> GetTopByEventAsync(string eventName, CancellationToken cancellationToken)
        => await GetTopByEventsAsync([eventName], cancellationToken);

    private async Task<List<TopPoiAnalyticsResponse>> GetTopByEventsAsync(IEnumerable<string> eventNames, CancellationToken cancellationToken)
    {
        var names = eventNames.Distinct(StringComparer.Ordinal).ToList();
        var results = await _context.AnalyticsEvents.Aggregate()
            .Match(x => names.Contains(x.EventName) && x.PoiId != null)
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

    private static FilterDefinition<AnalyticsEvent> BuildUsageHistoryFilter(UsageHistoryRequest request)
    {
        var builder = Builders<AnalyticsEvent>.Filter;
        var filter = FilterDefinition<AnalyticsEvent>.Empty;

        if (!string.IsNullOrWhiteSpace(request.EventName))
        {
            filter &= builder.Eq(x => x.EventName, request.EventName);
        }

        if (!string.IsNullOrWhiteSpace(request.PoiId))
        {
            filter &= builder.Eq(x => x.PoiId, request.PoiId);
        }

        if (!string.IsNullOrWhiteSpace(request.Lang))
        {
            filter &= builder.Eq(x => x.Lang, request.Lang);
        }

        return filter;
    }
}
