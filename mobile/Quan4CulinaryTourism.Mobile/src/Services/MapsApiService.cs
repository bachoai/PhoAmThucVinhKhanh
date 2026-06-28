using Quan4CulinaryTourism.Mobile.Models;

namespace Quan4CulinaryTourism.Mobile.Services;

public class MapsApiService
{
    private readonly HttpClient _httpClient;
    private readonly ApiClient _apiClient;

    public MapsApiService(ApiClient apiClient, HttpClient httpClient)
    {
        _apiClient = apiClient;
        _httpClient = httpClient;
    }

    public async Task<MapPackResponse?> GetPackManifestAsync(CancellationToken cancellationToken = default)
    {
        var response = await _apiClient.GetAsync<MapPackResponse?>("/api/v1/maps/pack-manifest", cancellationToken);
        return response.Success ? response.Data : null;
    }

    public async Task<string?> DownloadPackAsync(MapPackResponse mapPack, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(mapPack.DownloadUrl))
        {
            return null;
        }

        var folder = Path.Combine(FileSystem.AppDataDirectory, "maps");
        Directory.CreateDirectory(folder);

        var extension = Path.GetExtension(new Uri(Config.AppConfig.NormalizeUrl(mapPack.DownloadUrl)).AbsolutePath);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".zip";
        }

        var filePath = Path.Combine(folder, $"map-pack-{mapPack.Version}{extension}");
        using var stream = await _httpClient.GetStreamAsync(Config.AppConfig.NormalizeUrl(mapPack.DownloadUrl), cancellationToken);
        await using var target = File.Create(filePath);
        await stream.CopyToAsync(target, cancellationToken);
        return filePath;
    }
}
