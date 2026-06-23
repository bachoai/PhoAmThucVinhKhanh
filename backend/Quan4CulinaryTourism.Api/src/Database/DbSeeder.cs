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
        await SeedPoisAsync(cancellationToken);
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
        if (string.IsNullOrWhiteSpace(_defaultAdmin.Password))
        {
            return;
        }

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

    private async Task SeedPoisAsync(CancellationToken cancellationToken)
    {
        if (await _context.Pois.Find(FilterDefinition<Poi>.Empty).AnyAsync(cancellationToken))
        {
            return;
        }

        var categories = await _context.Categories.Find(FilterDefinition<Category>.Empty).ToListAsync(cancellationToken);
        var byCode = categories.ToDictionary(x => x.Code, x => x.Id);
        string CategoryId(string code) => byCode.TryGetValue(code, out var id) ? id : string.Empty;

        var hours = new List<OpeningHour>
        {
            new() { DayOfWeek = "Thứ Hai - Chủ Nhật", OpenTime = "10:00", CloseTime = "22:30" }
        };

        var pois = new[]
        {
            CreateDemoPoi("Ốc Oanh", "Quán ốc bình dân nổi tiếng với nhiều món xào bơ, rang muối và sốt me.", CategoryId("seafood"), "534 Vĩnh Khánh", 10.7592, 106.7045, "$$", 4.6, 1240, 10, "https://images.unsplash.com/photo-1559737558-2f5a35f4523b?auto=format&fit=crop&w=1200&q=85", ["hải sản", "vĩnh khánh", "buổi tối"], hours),
            CreateDemoPoi("Bánh mì chảo Cô 3 Hậu", "Bánh mì chảo nóng hổi, phù hợp cho bữa sáng và bữa trưa nhanh.", CategoryId("street_food"), "36 Nguyễn Hữu Hào", 10.7580, 106.7018, "$", 4.4, 630, 8, "https://images.unsplash.com/photo-1601050690597-df0568f70950?auto=format&fit=crop&w=1200&q=85", ["bánh mì", "bữa sáng", "địa phương"], hours),
            CreateDemoPoi("Cơm tấm Cây Điệp", "Cơm tấm sườn nướng thơm, phục vụ nhanh trong không khí Quận 4 thân thuộc.", CategoryId("rice"), "140/1 Đoàn Văn Bơ", 10.7548, 106.7049, "$", 4.5, 890, 9, "https://images.unsplash.com/photo-1515003197210-e0cd71810b5f?auto=format&fit=crop&w=1200&q=85", ["cơm tấm", "sườn nướng", "trưa"], hours),
            CreateDemoPoi("Súp cua Cô Bông", "Súp cua nóng với thịt cua, trứng cút và nấm, món ăn vặt quen thuộc.", CategoryId("street_food"), "22 Đoàn Văn Bơ", 10.7566, 106.7032, "$", 4.3, 410, 7, "https://images.unsplash.com/photo-1547592180-85f173990554?auto=format&fit=crop&w=1200&q=85", ["súp cua", "ăn vặt", "chiều tối"], hours),
            CreateDemoPoi("Cà phê bờ kênh Khánh Hội", "Không gian cà phê thoáng bên bờ kênh, thích hợp nghỉ chân sau hành trình khám phá.", CategoryId("coffee"), "Bến Vân Đồn", 10.7610, 106.7068, "$$", 4.2, 245, 5, "https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?auto=format&fit=crop&w=1200&q=85", ["cà phê", "view kênh", "thư giãn"], hours)
        };

        await _context.Pois.InsertManyAsync(pois, cancellationToken: cancellationToken);
    }

    private static Poi CreateDemoPoi(string name, string description, string categoryId, string address, double latitude, double longitude,
        string priceRange, double rating, int reviewCount, int priority, string imageUrl, List<string> tags, List<OpeningHour> hours) => new()
    {
        Name = name,
        Description = description,
        CategoryId = categoryId,
        Address = address,
        Ward = "Phường 13",
        District = "Quận 4",
        City = "TP. Hồ Chí Minh",
        Location = GeoLocationFactory.Create(longitude, latitude),
        PriceRange = priceRange,
        Rating = rating,
        ReviewCount = reviewCount,
        Priority = priority,
        Images = [new PoiImage { Url = imageUrl, IsThumbnail = true }],
        OpeningHours = hours,
        ContactInfo = new ContactInfo { Phone = "028 3826 0000" },
        Tags = tags,
        IsActive = true
    };
}
