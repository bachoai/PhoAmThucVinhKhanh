using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quan4CulinaryTourism.Mobile.DTOs;
using Quan4CulinaryTourism.Mobile.Models;
using Quan4CulinaryTourism.Mobile.Services;

namespace Quan4CulinaryTourism.Mobile.ViewModels;

public partial class QrEntryViewModel : BaseViewModel
{
    private readonly AnalyticsApiService _analyticsApiService;
    private readonly QrActivationApiService _qrActivationApiService;
    private readonly SemaphoreSlim _scanLock = new(1, 1);

    [ObservableProperty]
    private string poiCode = string.Empty;

    [ObservableProperty]
    private bool isScannerEnabled;

    [ObservableProperty]
    private string scannerStatus = "Quet QR hoac nhap ma kich hoat thu cong.";

    public QrEntryViewModel(AnalyticsApiService analyticsApiService, QrActivationApiService qrActivationApiService)
    {
        _analyticsApiService = analyticsApiService;
        _qrActivationApiService = qrActivationApiService;
        Title = "QR / Mã POI";
    }

    [RelayCommand]
    private async Task OpenPoiAsync() => await HandleCodeAsync(PoiCode, preferAutoplay: true);

    public void SetScannerAvailability(bool enabled, string status)
    {
        IsScannerEnabled = enabled;
        ScannerStatus = status;
    }

    public async Task HandleScannedCodeAsync(string? rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return;
        }

        PoiCode = rawValue.Trim();
        await HandleCodeAsync(PoiCode, preferAutoplay: true);
    }

    private async Task HandleCodeAsync(string? rawValue, bool preferAutoplay)
    {
        var value = rawValue?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(value))
        {
            SetError("Hãy nhập POI id hoặc mã QR demo.");
            return;
        }

        if (!await _scanLock.WaitAsync(0))
        {
            return;
        }

        try
        {
            SetError(string.Empty);
            var resolution = await ResolveCodeAsync(value);
            if (resolution is not null)
            {
                await _analyticsApiService.CollectAsync(new CollectAnalyticsRequest
                {
                    EventName = "qr_scanned",
                    PoiId = resolution.PoiId,
                    Metadata =
                    {
                        ["qrCode"] = resolution.Code,
                        ["scanMode"] = resolution.ScanMode,
                        ["activationTitle"] = resolution.Title
                    }
                });

                var autoplayMode = preferAutoplay ? resolution.ScanMode : string.Empty;
                await Shell.Current.GoToAsync($"poi-detail?id={Uri.EscapeDataString(resolution.PoiId)}&autoplay={Uri.EscapeDataString(autoplayMode)}&source=qr");
                return;
            }

            var poiId = ExtractPoiId(value);
            if (string.IsNullOrWhiteSpace(poiId))
            {
                SetError("Khong resolve duoc ma QR nay thanh POI.");
                return;
            }

            await _analyticsApiService.CollectAsync(new CollectAnalyticsRequest
            {
                EventName = "qr_scanned",
                PoiId = poiId,
                Metadata =
                {
                    ["fallback"] = "poi_id"
                }
            });

            var navigationUrl = preferAutoplay
                ? $"poi-detail?id={Uri.EscapeDataString(poiId)}&autoplay=prefer_audio&source=qr"
                : $"poi-detail?id={Uri.EscapeDataString(poiId)}";
            await Shell.Current.GoToAsync(navigationUrl);
        }
        finally
        {
            _scanLock.Release();
        }
    }

    private async Task<QrActivationResolveResponse?> ResolveCodeAsync(string value)
    {
        var code = value.StartsWith("quan4tourism://qr/", StringComparison.OrdinalIgnoreCase)
            ? value["quan4tourism://qr/".Length..]
            : value;
        return await _qrActivationApiService.ResolveAsync(code);
    }

    private static string? ExtractPoiId(string value)
    {
        if (value.StartsWith("quan4tourism://poi/", StringComparison.OrdinalIgnoreCase))
        {
            return value["quan4tourism://poi/".Length..];
        }

        return value.Length == 24 ? value : null;
    }
}
