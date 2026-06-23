using MongoDB.Bson;
using Quan4CulinaryTourism.Api.Database;
using Quan4CulinaryTourism.Api.DTOs;

namespace Quan4CulinaryTourism.Api.Services;

public class HealthService
{
    private readonly MongoDbContext _context;

    public HealthService(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<HealthResponse> CheckAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1), cancellationToken: cancellationToken);
        return new HealthResponse
        {
            Status = "Healthy",
            MongoConnected = true,
            ServerTimeUtc = DateTime.UtcNow
        };
    }
}
