namespace Quan4CulinaryTourism.Mobile.DTOs;

public class CollectAnalyticsRequest
{
    public string EventName { get; set; } = string.Empty;
    public string? AnonymousId { get; set; }
    public string? SessionId { get; set; }
    public string? PageViewId { get; set; }
    public string? PoiId { get; set; }
    public string? Lang { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = [];
}
