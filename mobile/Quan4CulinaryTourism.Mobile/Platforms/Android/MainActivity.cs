using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
namespace Quan4CulinaryTourism.Mobile;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
[IntentFilter(
    [Intent.ActionView],
    Categories = [Intent.CategoryDefault, Intent.CategoryBrowsable],
    DataScheme = "quan4tourism")]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        ForwardAppLink(Intent);
    }

    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);
        ForwardAppLink(intent);
    }

    private static void ForwardAppLink(Intent? intent)
    {
        var data = intent?.DataString;
        if (string.IsNullOrWhiteSpace(data) || !Uri.TryCreate(data, UriKind.Absolute, out var uri))
        {
            return;
        }

        Microsoft.Maui.Controls.Application.Current?.SendOnAppLinkRequestReceived(uri);
    }
}
