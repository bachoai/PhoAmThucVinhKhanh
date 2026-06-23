using SQLite;

namespace Quan4CulinaryTourism.Mobile.LocalDatabase;

[Table("cache_metadata")]
public class LocalCacheMetadata
{
    [PrimaryKey]
    public string CacheKey { get; set; } = string.Empty;
    public DateTime UpdatedAtUtc { get; set; }
}
