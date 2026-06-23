using SQLite;

namespace Quan4CulinaryTourism.Mobile.LocalDatabase;

[Table("categories")]
public class LocalCategory
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;
    public string JsonData { get; set; } = string.Empty;
}
