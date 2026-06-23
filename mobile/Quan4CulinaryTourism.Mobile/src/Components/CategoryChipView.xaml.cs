namespace Quan4CulinaryTourism.Mobile.Components;

public partial class CategoryChipView : ContentView
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(CategoryChipView), string.Empty);

    public CategoryChipView()
    {
        InitializeComponent();
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}
