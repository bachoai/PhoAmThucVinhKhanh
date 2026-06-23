using Quan4CulinaryTourism.Mobile.Models;

namespace Quan4CulinaryTourism.Mobile.Services;

public class MapsApiService
{
    private readonly ApiClient _apiClient;

    public MapsApiService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<MapPackResponse?> GetPackManifestAsync(CancellationToken cancellationToken = default)
    {
        var response = await _apiClient.GetAsync<MapPackResponse?>("/api/v1/maps/pack-manifest", cancellationToken);
        return response.Success ? response.Data : null;
    }
}
