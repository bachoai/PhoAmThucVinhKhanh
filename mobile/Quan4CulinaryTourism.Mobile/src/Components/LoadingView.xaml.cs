namespace Quan4CulinaryTourism.Mobile.Components;

public partial class LoadingView : ContentView
{
    public static readonly BindableProperty MessageProperty =
        BindableProperty.Create(nameof(Message), typeof(string), typeof(LoadingView), "Đang tải...");

    public LoadingView()
    {
        InitializeComponent();
    }

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }
}
