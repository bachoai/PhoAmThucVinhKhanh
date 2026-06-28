using SQLite;

namespace Quan4CulinaryTourism.Mobile.LocalDatabase;

[Table("map_packs")]
public class LocalMapPack
{
    [PrimaryKey]
    public string CacheKey { get; set; } = "active";

    public string JsonData { get; set; } = string.Empty;
}
