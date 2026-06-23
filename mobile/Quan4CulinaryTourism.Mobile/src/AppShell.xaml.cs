using Quan4CulinaryTourism.Mobile.Views;

namespace Quan4CulinaryTourism.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("poi-detail", typeof(PoiDetailPage));
        Routing.RegisterRoute("qr-entry", typeof(QrEntryPage));
    }
}
