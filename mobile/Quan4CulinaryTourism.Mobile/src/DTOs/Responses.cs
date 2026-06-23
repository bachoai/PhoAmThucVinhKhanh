using Quan4CulinaryTourism.Mobile.Models;

namespace Quan4CulinaryTourism.Mobile.DTOs;

public class AudioPackManifestResponse
{
    public string Version { get; set; } = string.Empty;
    public DateTime? GeneratedAt { get; set; }
    public List<AudioPackManifestItem> Items { get; set; } = [];
}

public class AudioPackManifestItem
{
    public string PoiId { get; set; } = string.Empty;
    public string PoiName { get; set; } = string.Empty;
    public List<PoiAudioResponse> Audios { get; set; } = [];
}
