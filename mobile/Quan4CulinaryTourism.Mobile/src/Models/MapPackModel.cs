namespace Quan4CulinaryTourism.Mobile.Models;

public class MapPackResponse
{
    public string Id { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
    public string EntryFile { get; set; } = "index.html";
    public string Sha256 { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public bool IsActive { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string? LocalPackagePath { get; set; }
    public string? ExtractedDirectoryPath { get; set; }
    public string? LocalEntryHtmlPath { get; set; }
    public DateTime? DownloadedAtUtc { get; set; }
}
