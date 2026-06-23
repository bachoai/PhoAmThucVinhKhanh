using Quan4CulinaryTourism.Mobile.Models;

namespace Quan4CulinaryTourism.Mobile.Components;

public partial class PoiCardView : ContentView
{
    public static readonly BindableProperty PoiProperty =
        BindableProperty.Create(nameof(Poi), typeof(PoiResponse), typeof(PoiCardView));

    public PoiCardView()
    {
        InitializeComponent();
    }

    public PoiResponse? Poi
    {
        get => (PoiResponse?)GetValue(PoiProperty);
        set => SetValue(PoiProperty, value);
    }
}
