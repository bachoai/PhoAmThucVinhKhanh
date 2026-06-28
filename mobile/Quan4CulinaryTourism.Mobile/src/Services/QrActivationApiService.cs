using Quan4CulinaryTourism.Mobile.Models;

namespace Quan4CulinaryTourism.Mobile.Services;

public class QrActivationApiService
{
    private readonly ApiClient _apiClient;

    public QrActivationApiService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<QrActivationResolveResponse?> ResolveAsync(string code, CancellationToken cancellationToken = default)
    {
        var response = await _apiClient.GetAsync<QrActivationResolveResponse>(
            $"/api/v1/qr-activations/resolve?code={Uri.EscapeDataString(code)}",
            cancellationToken);
        return response.Success ? response.Data : null;
    }
}
