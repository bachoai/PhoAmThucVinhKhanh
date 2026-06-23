using Microsoft.Maui.Controls;
using Quan4CulinaryTourism.Mobile.Config;

namespace Quan4CulinaryTourism.Mobile.Helpers;

public static class MediaHelper
{
    public const string PlaceholderImage = "poi_placeholder.svg";

    public static string NormalizeMediaUrl(string? url) => AppConfig.NormalizeUrl(url);

    public static ImageSource GetPoiImageSource(string? url)
    {
        var normalized = NormalizeMediaUrl(url);
        return string.IsNullOrWhiteSpace(normalized)
            ? ImageSource.FromFile(PlaceholderImage)
            : ImageSource.FromUri(new Uri(normalized));
    }

    public static string GetPoiImageUrlOrPlaceholder(string? url)
    {
        var normalized = NormalizeMediaUrl(url);
        return string.IsNullOrWhiteSpace(normalized) ? PlaceholderImage : normalized;
    }
}
