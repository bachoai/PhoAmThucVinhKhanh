namespace Quan4CulinaryTourism.Mobile.Models;

public class PoiAudioResponse
{
    public string Id { get; set; } = string.Empty;
    public string PoiId { get; set; } = string.Empty;
    public string Lang { get; set; } = string.Empty;
    public string AudioUrl { get; set; } = string.Empty;
    public string? VoiceName { get; set; }
    public string SourceType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public double DurationSeconds { get; set; }
    public long FileSizeBytes { get; set; }
    public string? LocalAudioPath { get; set; }
}
