using Quan4CulinaryTourism.Mobile.Models;

namespace Quan4CulinaryTourism.Mobile.Services;

public class CategoryApiService
{
    private readonly ApiClient _apiClient;

    public CategoryApiService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<CategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _apiClient.GetAsync<List<CategoryResponse>>("/api/v1/categories", cancellationToken);
        return response.Success ? response.Data ?? [] : [];
    }
}
