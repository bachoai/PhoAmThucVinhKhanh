namespace Quan4CulinaryTourism.Mobile.Models;

public sealed class OfflinePackStatus
{
    public bool HasCategories { get; set; }

    public bool HasPois { get; set; }

    public bool HasPoiDetails { get; set; }

    public bool HasOfflineAudio { get; set; }

    public bool HasMapPack { get; set; }

    public int CategoryCount { get; set; }

    public int PoiCount { get; set; }

    public int PoiDetailCount { get; set; }

    public int OfflineAudioCount { get; set; }

    public DateTime? LastPreparedAtUtc { get; set; }

    public bool IsReady => HasCategories && HasPois && HasPoiDetails;

    public string Summary =>
        $"POI {PoiCount} | chi tiet {PoiDetailCount} | audio offline {OfflineAudioCount} | map pack {(HasMapPack ? "co" : "chua co")}";
}

public sealed class OfflinePackPrepareOptions
{
    public string Language { get; set; } = "vi";

    public bool DownloadAudio { get; set; } = true;

    public bool DownloadMapPack { get; set; } = false;
}
