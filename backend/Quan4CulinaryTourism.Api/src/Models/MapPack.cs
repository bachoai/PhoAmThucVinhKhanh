namespace Quan4CulinaryTourism.Api.Models;

public class MapPack : BaseDocument
{
    public string Version { get; set; } = "v1";
    public string Name { get; set; } = "Default Pack";
    public string DownloadUrl { get; set; } = string.Empty;
    public string Sha256 { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public bool IsActive { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
