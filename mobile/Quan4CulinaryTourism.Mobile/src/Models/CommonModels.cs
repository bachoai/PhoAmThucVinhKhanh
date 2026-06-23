namespace Quan4CulinaryTourism.Mobile.Models;

public class PoiImage
{
    public string Url { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public bool IsThumbnail { get; set; }
}

public class OpeningHour
{
    public string DayOfWeek { get; set; } = string.Empty;
    public string OpenTime { get; set; } = string.Empty;
    public string CloseTime { get; set; } = string.Empty;
    public bool IsClosed { get; set; }
}

public class ContactInfo
{
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? FacebookUrl { get; set; }
    public string? WebsiteUrl { get; set; }
}

public class HealthResponse
{
    public string Status { get; set; } = string.Empty;
    public bool MongoConnected { get; set; }
    public DateTime ServerTimeUtc { get; set; }
}

public class AudioLanguageResponse
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class LanguageOption
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class ThemeOption
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class ApiEndpointOption
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
