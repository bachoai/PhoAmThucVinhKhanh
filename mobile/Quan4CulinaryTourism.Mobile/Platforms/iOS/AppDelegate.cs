using Foundation;
using Microsoft.Maui.Controls;
using UIKit;

namespace Quan4CulinaryTourism.Mobile;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
    {
        if (Uri.TryCreate(url.AbsoluteString, UriKind.Absolute, out var uri))
        {
            Application.Current?.SendOnAppLinkRequestReceived(uri);
        }

        return base.OpenUrl(application, url, options);
    }
}
