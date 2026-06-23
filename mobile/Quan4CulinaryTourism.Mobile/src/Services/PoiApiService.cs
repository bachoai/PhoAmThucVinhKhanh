using System.Web;
using Quan4CulinaryTourism.Mobile.Models;

namespace Quan4CulinaryTourism.Mobile.Services;

public class PoiApiService
{
    private readonly ApiClient _apiClient;

    public PoiApiService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<PoiResponse>> LoadAllAsync(string lang, string? keyword, string? categoryId, string? priceRange, CancellationToken cancellationToken = default)
    {
        var response = await _apiClient.GetAsync<List<PoiResponse>>(BuildPoiQuery("/api/v1/poi/load-all", lang, keyword, categoryId, priceRange), cancellationToken);
        return response.Success ? response.Data ?? [] : [];
    }

    public async Task<PoiDetailResponse?> GetByIdAsync(string id, string lang, CancellationToken cancellationToken = default)
    {
        var response = await _apiClient.GetAsync<PoiDetailResponse>($"/api/v1/poi/{id}?lang={Uri.EscapeDataString(lang)}", cancellationToken);
        return response.Success ? response.Data : null;
    }

    public async Task<List<NearbyPoiResponse>> GetNearbyAsync(double lat, double lng, int radius, int limit, string lang, CancellationToken cancellationToken = default)
    {
        var url = $"/api/v1/poi/nearby?lat={lat}&lng={lng}&radius={radius}&limit={limit}&lang={Uri.EscapeDataString(lang)}";
        var response = await _apiClient.GetAsync<List<NearbyPoiResponse>>(url, cancellationToken);
        return response.Success ? response.Data ?? [] : [];
    }

    public async Task<List<PoiResponse>> SearchAsync(string lang, string? keyword, string? categoryId, string? priceRange, CancellationToken cancellationToken = default)
    {
        var response = await _apiClient.GetAsync<List<PoiResponse>>(BuildPoiQuery("/api/v1/poi/search", lang, keyword, categoryId, priceRange), cancellationToken);
        return response.Success ? response.Data ?? [] : [];
    }

    private static string BuildPoiQuery(string basePath, string lang, string? keyword, string? categoryId, string? priceRange)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["lang"] = lang;

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query["keyword"] = keyword;
        }

        if (!string.IsNullOrWhiteSpace(categoryId))
        {
            query["categoryId"] = categoryId;
        }

        if (!string.IsNullOrWhiteSpace(priceRange))
        {
            query["priceRange"] = priceRange;
        }

        return $"{basePath}?{query}";
    }
}
