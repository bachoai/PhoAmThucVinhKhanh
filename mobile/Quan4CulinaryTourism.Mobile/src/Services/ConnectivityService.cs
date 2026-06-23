using Microsoft.Maui.Networking;

namespace Quan4CulinaryTourism.Mobile.Services;

public class ConnectivityService
{
    public ConnectivityService()
    {
        Connectivity.Current.ConnectivityChanged += (_, args) =>
        {
            ConnectivityChanged?.Invoke(this, args.NetworkAccess == NetworkAccess.Internet);
        };
    }

    public event EventHandler<bool>? ConnectivityChanged;

    public bool IsOnline() => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
}
