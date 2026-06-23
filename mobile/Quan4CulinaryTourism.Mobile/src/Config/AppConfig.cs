using Microsoft.Maui.Devices;

namespace Quan4CulinaryTourism.Mobile.Config;

public static class AppConfig
{
    private const string ApiBaseUrlKey = "api_base_url";

    public const string AndroidEmulatorBaseUrl = "http://10.0.2.2:5163";
    public const string WindowsBaseUrl = "http://localhost:5163";
    public const string LanExampleBaseUrl = "http://192.168.1.50:5163";
    public const string AndroidMapsApiKeyPlaceholder = "debug-placeholder-google-maps-key";
    public const int ApiTimeoutSeconds = 15;
    public const double DefaultLatitude = 10.7578;
    public const double DefaultLongitude = 106.7060;

    public static readonly string[] SupportedLanguages = ["vi", "en", "zh", "ja", "ko"];
    public static readonly string[] SupportedThemes = ["system", "light", "dark"];
    public static readonly string[] SupportedPriceRanges = ["", "$", "$$", "$$$"];
    public static readonly int[] NearbyRadiusOptions = [1000, 3000, 5000, 10000];

    public static string GetDefaultApiBaseUrl() =>
        DeviceInfo.Current.Platform == DevicePlatform.Android ? AndroidEmulatorBaseUrl : WindowsBaseUrl;

    public static string GetApiBaseUrl() =>
        Preferences.Default.Get(ApiBaseUrlKey, GetDefaultApiBaseUrl()).Trim().TrimEnd('/');

    public static void SetApiBaseUrl(string? url)
    {
        var value = string.IsNullOrWhiteSpace(url) ? GetDefaultApiBaseUrl() : url.Trim().TrimEnd('/');
        Preferences.Default.Set(ApiBaseUrlKey, value);
    }

    public static string NormalizeUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return string.Empty;
        }

        if (Uri.TryCreate(url, UriKind.Absolute, out var absolute))
        {
            return absolute.ToString();
        }

        return $"{GetApiBaseUrl()}/{url.TrimStart('/')}";
    }

    public static bool HasConfiguredMapsKey()
    {
#if ANDROID
        try
        {
            var context = Android.App.Application.Context;
            var appInfo = context.PackageManager?.GetApplicationInfo(context.PackageName!, Android.Content.PM.PackageInfoFlags.MetaData);
            var key = appInfo?.MetaData?.GetString("com.google.android.geo.API_KEY");
            return !string.IsNullOrWhiteSpace(key) && !string.Equals(key, AndroidMapsApiKeyPlaceholder, StringComparison.Ordinal);
        }
        catch
        {
            return false;
        }
#else
        return true;
#endif
    }
}
