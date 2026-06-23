using Quan4CulinaryTourism.Mobile.Models;

namespace Quan4CulinaryTourism.Mobile.Services;

public class HealthApiService
{
    private readonly ApiClient _apiClient;

    public HealthApiService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<HealthResponse?> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        var response = await _apiClient.GetAsync<HealthResponse>("/api/health", cancellationToken);
        return response.Success ? response.Data : null;
    }
}
