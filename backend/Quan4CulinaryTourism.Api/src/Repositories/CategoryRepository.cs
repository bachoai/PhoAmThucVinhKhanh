using MongoDB.Driver;
using Quan4CulinaryTourism.Api.Database;
using Quan4CulinaryTourism.Api.Models;

namespace Quan4CulinaryTourism.Api.Repositories;

public class CategoryRepository
{
    private readonly MongoDbContext _context;
    public CategoryRepository(MongoDbContext context) => _context = context;

    public Task<List<Category>> GetAllActiveAsync(CancellationToken cancellationToken = default) =>
        _context.Categories.Find(x => x.IsActive).SortBy(x => x.SortOrder).ToListAsync(cancellationToken);

    public Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _context.Categories.Find(FilterDefinition<Category>.Empty).SortBy(x => x.SortOrder).ToListAsync(cancellationToken);

    public async Task<Category?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        await _context.Categories.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);

    public async Task<Category?> GetByCodeAsync(string code, CancellationToken cancellationToken = default) =>
        await _context.Categories.Find(x => x.Code == code).FirstOrDefaultAsync(cancellationToken);

    public async Task CreateAsync(Category category, CancellationToken cancellationToken = default) =>
        await _context.Categories.InsertOneAsync(category, cancellationToken: cancellationToken);

    public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        category.UpdatedAt = DateTime.UtcNow;
        await _context.Categories.ReplaceOneAsync(x => x.Id == category.Id, category, cancellationToken: cancellationToken);
    }

    public async Task SoftDeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var update = Builders<Category>.Update.Set(x => x.IsActive, false).Set(x => x.UpdatedAt, DateTime.UtcNow);
        await _context.Categories.UpdateOneAsync(x => x.Id == id, update, cancellationToken: cancellationToken);
    }
}
