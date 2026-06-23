namespace Quan4CulinaryTourism.Mobile.Components;

public partial class EmptyStateView : ContentView
{
    public static readonly BindableProperty TitleTextProperty =
        BindableProperty.Create(nameof(TitleText), typeof(string), typeof(EmptyStateView), "Không có dữ liệu");

    public static readonly BindableProperty DescriptionTextProperty =
        BindableProperty.Create(nameof(DescriptionText), typeof(string), typeof(EmptyStateView), "Hãy thử lại sau.");

    public EmptyStateView()
    {
        InitializeComponent();
    }

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public string DescriptionText
    {
        get => (string)GetValue(DescriptionTextProperty);
        set => SetValue(DescriptionTextProperty, value);
    }
}
