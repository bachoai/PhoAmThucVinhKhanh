using MongoDB.Driver;
using Quan4CulinaryTourism.Api.Database;
using Quan4CulinaryTourism.Api.Models;

namespace Quan4CulinaryTourism.Api.Repositories;

public class MapPackRepository
{
    private readonly MongoDbContext _context;
    public MapPackRepository(MongoDbContext context) => _context = context;

    public async Task<MapPack?> GetActiveAsync(CancellationToken cancellationToken = default) =>
        await _context.MapPacks.Find(x => x.IsActive).SortByDescending(x => x.CreatedAt).FirstOrDefaultAsync(cancellationToken);

    public async Task CreateAsync(MapPack entity, CancellationToken cancellationToken = default) =>
        await _context.MapPacks.InsertOneAsync(entity, cancellationToken: cancellationToken);

    public async Task SetActiveAsync(string id, CancellationToken cancellationToken = default)
    {
        await _context.MapPacks.UpdateManyAsync(FilterDefinition<MapPack>.Empty, Builders<MapPack>.Update.Set(x => x.IsActive, false), cancellationToken: cancellationToken);
        await _context.MapPacks.UpdateOneAsync(x => x.Id == id, Builders<MapPack>.Update.Set(x => x.IsActive, true), cancellationToken: cancellationToken);
    }
}
