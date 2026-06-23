using SQLite;

namespace Quan4CulinaryTourism.Mobile.LocalDatabase;

[Table("poi_audios")]
public class LocalPoiAudio
{
    [PrimaryKey]
    public string CacheKey { get; set; } = string.Empty;
    public string JsonData { get; set; } = string.Empty;
}
