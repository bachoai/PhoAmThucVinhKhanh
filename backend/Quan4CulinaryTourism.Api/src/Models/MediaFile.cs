namespace Quan4CulinaryTourism.Api.Models;

public class MediaFile : BaseDocument
{
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string StorageProvider { get; set; } = "local";
    public string? BucketName { get; set; }
    public string? ObjectKey { get; set; }
    public string? UploadedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
