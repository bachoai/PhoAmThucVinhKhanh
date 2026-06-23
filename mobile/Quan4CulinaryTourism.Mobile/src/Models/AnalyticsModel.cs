namespace Quan4CulinaryTourism.Mobile.Models;

public class AnalyticsEvent
{
    public string EventName { get; set; } = string.Empty;
    public string? PoiId { get; set; }
    public string Lang { get; set; } = "vi";
    public Dictionary<string, object> Metadata { get; set; } = [];
}
