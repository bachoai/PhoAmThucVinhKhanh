using MongoDB.Driver;
using Quan4CulinaryTourism.Api.Database;
using Quan4CulinaryTourism.Api.Models;

namespace Quan4CulinaryTourism.Api.Repositories;

public class AuditLogRepository
{
    private readonly MongoDbContext _context;
    public AuditLogRepository(MongoDbContext context) => _context = context;

    public async Task CreateAsync(AuditLog entity, CancellationToken cancellationToken = default) =>
        await _context.AuditLogs.InsertOneAsync(entity, cancellationToken: cancellationToken);

    public Task<List<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _context.AuditLogs.Find(FilterDefinition<AuditLog>.Empty).SortByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
}
