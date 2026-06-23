using SQLite;

namespace Quan4CulinaryTourism.Mobile.LocalDatabase;

[Table("pois")]
public class LocalPoi
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;
    public string JsonData { get; set; } = string.Empty;
}
