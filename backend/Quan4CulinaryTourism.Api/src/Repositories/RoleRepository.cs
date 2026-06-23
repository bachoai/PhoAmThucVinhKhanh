using MongoDB.Driver;
using Quan4CulinaryTourism.Api.Database;
using Quan4CulinaryTourism.Api.Models;

namespace Quan4CulinaryTourism.Api.Repositories;

public class RoleRepository
{
    private readonly MongoDbContext _context;
    public RoleRepository(MongoDbContext context) => _context = context;

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default) =>
        await _context.Roles.Find(x => x.Name == name).FirstOrDefaultAsync(cancellationToken);

    public async Task CreateAsync(Role role, CancellationToken cancellationToken = default) =>
        await _context.Roles.InsertOneAsync(role, cancellationToken: cancellationToken);

    public Task<List<Role>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _context.Roles.Find(FilterDefinition<Role>.Empty).ToListAsync(cancellationToken);
}
