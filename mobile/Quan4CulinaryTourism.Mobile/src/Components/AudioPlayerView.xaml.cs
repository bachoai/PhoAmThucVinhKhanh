namespace Quan4CulinaryTourism.Mobile.Components;

public partial class AudioPlayerView : ContentView
{
    public static readonly BindableProperty StateProperty =
        BindableProperty.Create(nameof(State), typeof(string), typeof(AudioPlayerView), "Idle");

    public static readonly BindableProperty MessageProperty =
        BindableProperty.Create(nameof(Message), typeof(string), typeof(AudioPlayerView), string.Empty);

    public static readonly BindableProperty PlayCommandProperty =
        BindableProperty.Create(nameof(PlayCommand), typeof(Command), typeof(AudioPlayerView));

    public static readonly BindableProperty PauseCommandProperty =
        BindableProperty.Create(nameof(PauseCommand), typeof(Command), typeof(AudioPlayerView));

    public static readonly BindableProperty StopCommandProperty =
        BindableProperty.Create(nameof(StopCommand), typeof(Command), typeof(AudioPlayerView));

    public AudioPlayerView()
    {
        InitializeComponent();
    }

    public string State
    {
        get => (string)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public Command? PlayCommand
    {
        get => (Command?)GetValue(PlayCommandProperty);
        set => SetValue(PlayCommandProperty, value);
    }

    public Command? PauseCommand
    {
        get => (Command?)GetValue(PauseCommandProperty);
        set => SetValue(PauseCommandProperty, value);
    }

    public Command? StopCommand
    {
        get => (Command?)GetValue(StopCommandProperty);
        set => SetValue(StopCommandProperty, value);
    }
}
