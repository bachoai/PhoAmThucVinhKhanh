using MongoDB.Driver;
using Quan4CulinaryTourism.Api.Database;
using Quan4CulinaryTourism.Api.Models;

namespace Quan4CulinaryTourism.Api.Repositories;

public class MediaFileRepository
{
    private readonly MongoDbContext _context;
    public MediaFileRepository(MongoDbContext context) => _context = context;

    public async Task CreateAsync(MediaFile entity, CancellationToken cancellationToken = default) =>
        await _context.MediaFiles.InsertOneAsync(entity, cancellationToken: cancellationToken);

    public async Task<MediaFile?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        await _context.MediaFiles.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
}
