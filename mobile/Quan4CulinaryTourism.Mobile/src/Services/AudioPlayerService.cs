using Plugin.Maui.Audio;
using Quan4CulinaryTourism.Mobile.Config;

namespace Quan4CulinaryTourism.Mobile.Services;

public class AudioPlayerService
{
    private readonly IAudioManager _audioManager;
    private readonly HttpClient _httpClient;
    private IAudioPlayer? _player;
    private MemoryStream? _currentBuffer;
    private Stream? _currentStream;

    public AudioPlayerService(IAudioManager audioManager, HttpClient httpClient)
    {
        _audioManager = audioManager;
        _httpClient = httpClient;
    }

    public bool IsPlaying => _player?.IsPlaying ?? false;

    public async Task PlayAsync(string? audioUrl, string? localAudioPath = null)
    {
        await StopAsync();

        if (!string.IsNullOrWhiteSpace(localAudioPath) && File.Exists(localAudioPath))
        {
            _currentStream = File.OpenRead(localAudioPath);
            _player = _audioManager.CreatePlayer(_currentStream);
            _player.Play();
            return;
        }

        if (string.IsNullOrWhiteSpace(audioUrl))
        {
            throw new InvalidOperationException("Địa điểm này chưa có audio thuyết minh.");
        }

        var stream = await _httpClient.GetStreamAsync(AppConfig.NormalizeUrl(audioUrl));
        _currentBuffer = new MemoryStream();
        await stream.CopyToAsync(_currentBuffer);
        _currentBuffer.Position = 0;

        _player = _audioManager.CreatePlayer(_currentBuffer);
        _player.Play();
    }

    public Task PauseAsync()
    {
        _player?.Pause();
        return Task.CompletedTask;
    }

    public Task ResumeAsync()
    {
        _player?.Play();
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        _player?.Stop();
        _player?.Dispose();
        _player = null;
        _currentBuffer?.Dispose();
        _currentBuffer = null;
        _currentStream?.Dispose();
        _currentStream = null;
        return Task.CompletedTask;
    }
}
