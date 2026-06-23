using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quan4CulinaryTourism.Mobile.Models;
using Quan4CulinaryTourism.Mobile.Services;

namespace Quan4CulinaryTourism.Mobile.ViewModels;

public partial class SplashViewModel : BaseViewModel
{
    private readonly SettingsService _settingsService;

    [ObservableProperty]
    private bool showLanguageSelector;

    [ObservableProperty]
    private LanguageOption? selectedLanguageOption;

    public SplashViewModel(SettingsService settingsService)
    {
        _settingsService = settingsService;
        Title = "Quan4 Culinary Tourism";
        Languages = _settingsService.GetLanguages();
        SelectedLanguageOption = Languages.FirstOrDefault(language => language.Code == _settingsService.GetLanguage()) ?? Languages.First();
    }

    public IReadOnlyList<LanguageOption> Languages { get; }

    public async Task InitializeAsync()
    {
        await Task.Delay(1000);
        ShowLanguageSelector = _settingsService.IsFirstLaunch();

        if (!ShowLanguageSelector)
        {
            await Shell.Current.GoToAsync("//home");
        }
    }

    [RelayCommand]
    private async Task ContinueAsync()
    {
        _settingsService.SetLanguage(SelectedLanguageOption?.Code ?? "vi");
        ShowLanguageSelector = false;
        await Shell.Current.GoToAsync("//home");
    }
}
