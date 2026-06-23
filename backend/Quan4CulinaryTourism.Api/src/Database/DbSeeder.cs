using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Quan4CulinaryTourism.Api.Helpers;
using Quan4CulinaryTourism.Api.Models;

namespace Quan4CulinaryTourism.Api.Database;

public class DbSeeder
{
    private readonly MongoDbContext _context;
    private readonly DefaultAdminSettings _defaultAdmin;
    private readonly PasswordHasher _passwordHasher;

    public DbSeeder(MongoDbContext context, IOptions<DefaultAdminSettings> defaultAdmin, PasswordHasher passwordHasher)
    {
        _context = context;
        _defaultAdmin = defaultAdmin.Value;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedRolesAsync(cancellationToken);
        await SeedAdminAsync(cancellationToken);
        await SeedCategoriesAsync(cancellationToken);
        await SeedMapPackAsync(cancellationToken);
    }

    private async Task SeedRolesAsync(CancellationToken cancellationToken)
    {
        var roles = new[]
        {
            new Role { Name = SharedConstants.UserRoles.Admin, Description = "System administrator" },
            new Role { Name = SharedConstants.UserRoles.Owner, Description = "Business owner" },
            new Role { Name = SharedConstants.UserRoles.User, Description = "End user" }
        };

        foreach (var role in roles)
        {
            var exists = await _context.Roles.Find(x => x.Name == role.Name).AnyAsync(cancellationToken);
            if (!exists)
            {
                await _context.Roles.InsertOneAsync(role, cancellationToken: cancellationToken);
            }
        }
    }

    private async Task SeedAdminAsync(CancellationToken cancellationToken)
    {
        var existing = await _context.Users.Find(x => x.Email == _defaultAdmin.Email).FirstOrDefaultAsync(cancellationToken);
        if (existing is not null)
        {
            return;
        }

        var admin = new User
        {
            FullName = _defaultAdmin.FullName,
            Email = _defaultAdmin.Email.ToLowerInvariant(),
            PasswordHash = _passwordHasher.HashPassword(_defaultAdmin.Password),
            Roles = [SharedConstants.UserRoles.Admin, SharedConstants.UserRoles.User],
            OwnerStatus = SharedConstants.OwnerApproved,
            EmailVerified = true,
            IsActive = true
        };

        await _context.Users.InsertOneAsync(admin, cancellationToken: cancellationToken);
    }

    private async Task SeedCategoriesAsync(CancellationToken cancellationToken)
    {
        if (await _context.Categories.Find(FilterDefinition<Category>.Empty).AnyAsync(cancellationToken))
        {
            return;
        }

        var categories = new[]
        {
            new Category { Code = "street_food", Name = "Ăn vặt", SortOrder = 1 },
            new Category { Code = "rice", Name = "Cơm", SortOrder = 2 },
            new Category { Code = "noodles", Name = "Bún / Phở / Mì", SortOrder = 3 },
            new Category { Code = "seafood", Name = "Hải sản", SortOrder = 4 },
            new Category { Code = "coffee", Name = "Cà phê", SortOrder = 5 },
            new Category { Code = "dessert", Name = "Tráng miệng", SortOrder = 6 },
            new Category { Code = "drink", Name = "Đồ uống", SortOrder = 7 },
            new Category { Code = "hotpot_bbq", Name = "Lẩu / Nướng", SortOrder = 8 },
            new Category { Code = "vietnamese_food", Name = "Món Việt", SortOrder = 9 },
            new Category { Code = "night_food", Name = "Quán đêm", SortOrder = 10 }
        };

        await _context.Categories.InsertManyAsync(categories, cancellationToken: cancellationToken);
    }

    private async Task SeedMapPackAsync(CancellationToken cancellationToken)
    {
        var exists = await _context.MapPacks.Find(FilterDefinition<MapPack>.Empty).AnyAsync(cancellationToken);
        if (exists)
        {
            return;
        }

        await _context.MapPacks.InsertOneAsync(new MapPack
        {
            Version = "v1",
            Name = "Default Quan 4 Map Pack",
            DownloadUrl = string.Empty,
            Sha256 = string.Empty,
            IsActive = true,
            PublishedAt = DateTime.UtcNow
        }, cancellationToken: cancellationToken);
    }
}
