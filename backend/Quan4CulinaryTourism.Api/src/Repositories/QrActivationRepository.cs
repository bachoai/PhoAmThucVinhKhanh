using MongoDB.Driver;
using Quan4CulinaryTourism.Api.Database;
using Quan4CulinaryTourism.Api.Models;

namespace Quan4CulinaryTourism.Api.Repositories;

public class QrActivationRepository
{
    private readonly MongoDbContext _context;

    public QrActivationRepository(MongoDbContext context) => _context = context;

    public Task<List<QrActivation>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _context.QrActivations.Find(FilterDefinition<QrActivation>.Empty)
            .SortBy(x => x.StopZone)
            .ThenBy(x => x.SortOrder)
            .ThenBy(x => x.Title)
            .ThenByDescending(x => x.UpdatedAt)
            .ToListAsync(cancellationToken);

    public async Task<QrActivation?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        await _context.QrActivations.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);

    public async Task<QrActivation?> GetByCodeAsync(string code, CancellationToken cancellationToken = default) =>
        await _context.QrActivations.Find(x => x.Code == code).FirstOrDefaultAsync(cancellationToken);

    public Task CreateAsync(QrActivation entity, CancellationToken cancellationToken = default) =>
        _context.QrActivations.InsertOneAsync(entity, cancellationToken: cancellationToken);

    public Task UpdateAsync(QrActivation entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        return _context.QrActivations.ReplaceOneAsync(x => x.Id == entity.Id, entity, cancellationToken: cancellationToken);
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default) =>
        _context.QrActivations.DeleteOneAsync(x => x.Id == id, cancellationToken);
}
