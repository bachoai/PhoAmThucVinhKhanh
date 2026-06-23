namespace Quan4CulinaryTourism.Mobile.Components;

public partial class OfflineBannerView : ContentView
{
    public static readonly BindableProperty IsOfflineProperty =
        BindableProperty.Create(nameof(IsOffline), typeof(bool), typeof(OfflineBannerView), false);

    public OfflineBannerView()
    {
        InitializeComponent();
    }

    public bool IsOffline
    {
        get => (bool)GetValue(IsOfflineProperty);
        set => SetValue(IsOfflineProperty, value);
    }
}
