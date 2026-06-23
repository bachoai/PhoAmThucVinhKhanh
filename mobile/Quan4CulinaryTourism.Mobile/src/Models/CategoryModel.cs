namespace Quan4CulinaryTourism.Mobile.Models;

public class CategoryResponse
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}
