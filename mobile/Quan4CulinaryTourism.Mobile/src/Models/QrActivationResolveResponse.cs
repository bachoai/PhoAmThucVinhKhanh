namespace Quan4CulinaryTourism.Mobile.Models;

public class QrActivationResolveResponse
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string PoiId { get; set; } = string.Empty;
    public string PoiName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ScanMode { get; set; } = "prefer_audio";
    public string DeepLink { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime UpdatedAt { get; set; }
}
