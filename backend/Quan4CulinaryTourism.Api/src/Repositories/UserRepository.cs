using MongoDB.Driver;
using Quan4CulinaryTourism.Api.Database;
using Quan4CulinaryTourism.Api.Models;

namespace Quan4CulinaryTourism.Api.Repositories;

public class UserRepository
{
    private readonly MongoDbContext _context;
    public UserRepository(MongoDbContext context) => _context = context;

    public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        await _context.Users.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await _context.Users.Find(x => x.Email == email.ToLowerInvariant()).FirstOrDefaultAsync(cancellationToken);

    public async Task CreateAsync(User user, CancellationToken cancellationToken = default) =>
        await _context.Users.InsertOneAsync(user, cancellationToken: cancellationToken);

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        user.UpdatedAt = DateTime.UtcNow;
        await _context.Users.ReplaceOneAsync(x => x.Id == user.Id, user, cancellationToken: cancellationToken);
    }

    public Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _context.Users.Find(FilterDefinition<User>.Empty).SortByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);

    public Task<long> CountAsync(FilterDefinition<User>? filter = null, CancellationToken cancellationToken = default) =>
        _context.Users.CountDocumentsAsync(filter ?? FilterDefinition<User>.Empty, cancellationToken: cancellationToken);
}
