using SQLite;

namespace Quan4CulinaryTourism.Mobile.LocalDatabase;

[Table("poi_details")]
public class LocalPoiDetail
{
    [PrimaryKey]
    public string PoiId { get; set; } = string.Empty;
    public string JsonData { get; set; } = string.Empty;
}
