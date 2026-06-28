using Quan4CulinaryTourism.Mobile.Services;

namespace Quan4CulinaryTourism.Mobile;

public partial class App : Application
{
    private readonly AppShell _shell;
    private readonly SettingsService _settingsService;
    private readonly GeofenceService _geofenceService;

    public App(AppShell shell, SettingsService settingsService, GeofenceService geofenceService)
    {
        InitializeComponent();
        _shell = shell;
        _settingsService = settingsService;
        _geofenceService = geofenceService;

        ApplyTheme(_settingsService.GetTheme());
        _settingsService.ThemeChanged += (_, theme) => ApplyTheme(theme);
        _geofenceService.EnsureStarted();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(_shell);
    }

    protected override async void OnAppLinkRequestReceived(Uri uri)
    {
        base.OnAppLinkRequestReceived(uri);

        var route = BuildAppLinkRoute(uri);
        if (string.IsNullOrWhiteSpace(route))
        {
            return;
        }

        await MainThread.InvokeOnMainThreadAsync(() => _shell.GoToAsync(route));
    }

    private void ApplyTheme(string theme)
    {
        UserAppTheme = theme.ToLowerInvariant() switch
        {
            "light" => AppTheme.Light,
            "dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
    }

    private static string? BuildAppLinkRoute(Uri? uri)
    {
        if (uri is null || !uri.Scheme.Equals("quan4tourism", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var value = uri.AbsolutePath.Trim('/');
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return uri.Host.ToLowerInvariant() switch
        {
            "poi" => $"poi-detail?id={Uri.EscapeDataString(value)}&autoplay=prefer_audio&source=qr",
            "qr" => $"qr-entry?code={Uri.EscapeDataString(value)}",
            _ => null
        };
    }
}
