namespace Quan4CulinaryTourism.Mobile.Models;

public class PoiDetailResponse : PoiResponse
{
    public List<OpeningHour> OpeningHours { get; set; } = [];
    public ContactInfo? ContactInfo { get; set; }
    public string? OwnerId { get; set; }
    public string AudioStatus { get; set; } = string.Empty;
}
