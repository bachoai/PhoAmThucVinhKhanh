using MongoDB.Driver;
using Quan4CulinaryTourism.Api.Database;
using Quan4CulinaryTourism.Api.Models;

namespace Quan4CulinaryTourism.Api.Repositories;

public class AudioTaskRepository
{
    private readonly MongoDbContext _context;
    public AudioTaskRepository(MongoDbContext context) => _context = context;
    public async Task CreateAsync(AudioTask entity, CancellationToken cancellationToken = default) =>
        await _context.AudioTasks.InsertOneAsync(entity, cancellationToken: cancellationToken);
}
