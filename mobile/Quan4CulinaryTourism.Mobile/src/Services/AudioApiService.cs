using Quan4CulinaryTourism.Mobile.DTOs;
using Quan4CulinaryTourism.Mobile.Models;

namespace Quan4CulinaryTourism.Mobile.Services;

public class AudioApiService
{
    private readonly ApiClient _apiClient;

    public AudioApiService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<AudioLanguageResponse>> GetLanguagesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _apiClient.GetAsync<List<AudioLanguageResponse>>("/api/v1/audio/languages", cancellationToken);
        return response.Success ? response.Data ?? [] : [];
    }

    public async Task<PoiAudioResponse?> GetPoiAudioAsync(string poiId, string lang, CancellationToken cancellationToken = default)
    {
        var response = await _apiClient.GetAsync<PoiAudioResponse?>($"/api/v1/poi/{poiId}/audio?lang={Uri.EscapeDataString(lang)}", cancellationToken);
        return response.Success ? response.Data : null;
    }

    public async Task<AudioPackManifestResponse?> GetPackManifestAsync(CancellationToken cancellationToken = default)
    {
        var response = await _apiClient.GetAsync<AudioPackManifestResponse>("/api/v1/audio/pack-manifest", cancellationToken);
        return response.Success ? response.Data : null;
    }
}
