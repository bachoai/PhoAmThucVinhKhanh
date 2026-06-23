using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quan4CulinaryTourism.Mobile.DTOs;
using Quan4CulinaryTourism.Mobile.Services;

namespace Quan4CulinaryTourism.Mobile.ViewModels;

public partial class QrEntryViewModel : BaseViewModel
{
    private readonly AnalyticsApiService _analyticsApiService;

    [ObservableProperty]
    private string poiCode = string.Empty;

    public QrEntryViewModel(AnalyticsApiService analyticsApiService)
    {
        _analyticsApiService = analyticsApiService;
        Title = "QR / Mã POI";
    }

    [RelayCommand]
    private async Task OpenPoiAsync()
    {
        var value = PoiCode?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(value))
        {
            SetError("Hãy nhập POI id hoặc mã QR demo.");
            return;
        }

        var poiId = value.StartsWith("quan4tourism://poi/", StringComparison.OrdinalIgnoreCase)
            ? value["quan4tourism://poi/".Length..]
            : value;

        await _analyticsApiService.CollectAsync(new CollectAnalyticsRequest
        {
            EventName = "qr_scanned",
            PoiId = poiId
        });

        await Shell.Current.GoToAsync($"poi-detail?id={Uri.EscapeDataString(poiId)}");
    }
}
