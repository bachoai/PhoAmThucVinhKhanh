using MongoDB.Bson.Serialization.Attributes;

namespace Quan4CulinaryTourism.Api.Models;

public class AnalyticsEvent : BaseDocument
{
    public string? AnonymousId { get; set; }
    public string? SessionId { get; set; }
    public string? PageViewId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string? PoiId { get; set; }
    public string? Lang { get; set; }

    [BsonDictionaryOptions(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.Document)]
    public Dictionary<string, object> Metadata { get; set; } = [];

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
