using MongoDB.Bson.Serialization.Attributes;

namespace Quan4CulinaryTourism.Api.Models;

public class AuditLog : BaseDocument
{
    public string? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public string? ResourceId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    [BsonDictionaryOptions(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.Document)]
    public Dictionary<string, object> Details { get; set; } = [];

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
