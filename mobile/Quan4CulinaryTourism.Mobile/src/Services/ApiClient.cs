using System.Text;
using System.Text.Json;
using Quan4CulinaryTourism.Mobile.Config;
using Quan4CulinaryTourism.Mobile.Models;

namespace Quan4CulinaryTourism.Mobile.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly SettingsService _settingsService;
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiClient(HttpClient httpClient, SettingsService settingsService)
    {
        _httpClient = httpClient;
        _settingsService = settingsService;
        _httpClient.Timeout = TimeSpan.FromSeconds(AppConfig.ApiTimeoutSeconds);
    }

    public async Task<ApiResponse<T>> GetAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(BuildUri(url), cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = $"Yêu cầu thất bại ({(int)response.StatusCode}).",
                    Errors = [content]
                };
            }

            var result = JsonSerializer.Deserialize<ApiResponse<T>>(content, _serializerOptions);
            return result ?? new ApiResponse<T> { Success = false, Message = "Không đọc được dữ liệu phản hồi." };
        }
        catch (TaskCanceledException)
        {
            return new ApiResponse<T> { Success = false, Message = "Kết nối tới máy chủ bị timeout." };
        }
        catch (HttpRequestException ex)
        {
            return new ApiResponse<T> { Success = false, Message = "Không thể kết nối backend.", Errors = [ex.Message] };
        }
        catch (JsonException ex)
        {
            return new ApiResponse<T> { Success = false, Message = "Dữ liệu trả về không đúng định dạng.", Errors = [ex.Message] };
        }
    }

    public async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest body, CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = JsonSerializer.Serialize(body, _serializerOptions);
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(BuildUri(url), content, cancellationToken);
            var raw = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<TResponse>
                {
                    Success = false,
                    Message = $"Yêu cầu thất bại ({(int)response.StatusCode}).",
                    Errors = [raw]
                };
            }

            var result = JsonSerializer.Deserialize<ApiResponse<TResponse>>(raw, _serializerOptions);
            return result ?? new ApiResponse<TResponse> { Success = false, Message = "Không đọc được dữ liệu phản hồi." };
        }
        catch (TaskCanceledException)
        {
            return new ApiResponse<TResponse> { Success = false, Message = "Kết nối tới máy chủ bị timeout." };
        }
        catch (HttpRequestException ex)
        {
            return new ApiResponse<TResponse> { Success = false, Message = "Không thể kết nối backend.", Errors = [ex.Message] };
        }
        catch (JsonException ex)
        {
            return new ApiResponse<TResponse> { Success = false, Message = "Dữ liệu trả về không đúng định dạng.", Errors = [ex.Message] };
        }
    }

    private Uri BuildUri(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var absolute))
        {
            return absolute;
        }

        var baseUrl = _settingsService.GetApiBaseUrl();
        return new Uri(new Uri(baseUrl.TrimEnd('/') + "/"), url.TrimStart('/'));
    }
}
