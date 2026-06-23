using Quan4CulinaryTourism.Mobile.Models;

namespace Quan4CulinaryTourism.Mobile.Services;

public class AudioDownloadService
{
    private readonly HttpClient _httpClient;
    private readonly OfflineDatabaseService _offlineDatabaseService;

    public AudioDownloadService(HttpClient httpClient, OfflineDatabaseService offlineDatabaseService)
    {
        _httpClient = httpClient;
        _offlineDatabaseService = offlineDatabaseService;
    }

    public async Task<string> DownloadAsync(PoiAudioResponse audio, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(audio.AudioUrl))
        {
            throw new InvalidOperationException("POI này chưa có audio để tải offline.");
        }

        var folder = Path.Combine(FileSystem.AppDataDirectory, "audio");
        Directory.CreateDirectory(folder);

        var extension = Path.GetExtension(new Uri(Config.AppConfig.NormalizeUrl(audio.AudioUrl)).AbsolutePath);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".mp3";
        }

        var filePath = Path.Combine(folder, $"{audio.PoiId}_{audio.Lang}{extension}");
        using var stream = await _httpClient.GetStreamAsync(Config.AppConfig.NormalizeUrl(audio.AudioUrl), cancellationToken);
        await using var target = File.Create(filePath);
        await stream.CopyToAsync(target, cancellationToken);

        audio.LocalAudioPath = filePath;
        await _offlineDatabaseService.SavePoiAudioAsync(audio);

        return filePath;
    }
}
