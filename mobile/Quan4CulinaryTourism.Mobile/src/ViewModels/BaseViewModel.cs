using CommunityToolkit.Mvvm.ComponentModel;

namespace Quan4CulinaryTourism.Mobile.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private string title = string.Empty;

    protected void SetError(string message)
    {
        ErrorMessage = message;
        HasError = !string.IsNullOrWhiteSpace(message);
    }

    protected async Task RunBusyAsync(Func<Task> action, string? errorMessage = null)
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            SetError(string.Empty);
            await action();
        }
        catch (Exception ex)
        {
            SetError(errorMessage ?? ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
