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

    private void ApplyTheme(string theme)
    {
        UserAppTheme = theme.ToLowerInvariant() switch
        {
            "light" => AppTheme.Light,
            "dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
    }
}
