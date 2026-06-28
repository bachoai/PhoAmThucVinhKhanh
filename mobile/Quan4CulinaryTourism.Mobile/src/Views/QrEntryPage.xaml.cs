using Quan4CulinaryTourism.Mobile.ViewModels;
using ZXing.Net.Maui;

namespace Quan4CulinaryTourism.Mobile.Views;

public partial class QrEntryPage : ContentPage, IQueryAttributable
{
    private readonly QrEntryViewModel _viewModel;
    private string? _pendingCode;

    public QrEntryPage(QrEntryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        CameraView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.TwoDimensional,
            AutoRotate = true,
            Multiple = false
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await EnsureCameraAccessAsync();

        if (string.IsNullOrWhiteSpace(_pendingCode))
        {
            return;
        }

        var code = _pendingCode;
        _pendingCode = null;
        await _viewModel.HandleScannedCodeAsync(code);
    }

    protected override void OnDisappearing()
    {
        CameraView.IsDetecting = false;
        base.OnDisappearing();
    }

    private async Task EnsureCameraAccessAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Camera>();
        }

        var granted = status == PermissionStatus.Granted;
        CameraView.IsDetecting = granted;
        _viewModel.SetScannerAvailability(
            granted,
            granted
                ? "Huong camera vao ma QR de mo noi dung ngay lap tuc."
                : "Camera chua duoc cap quyen. Ban van co the nhap ma kich hoat thu cong.");
    }

    private async void OnBarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
    {
        var value = e.Results.FirstOrDefault()?.Value;
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        CameraView.IsDetecting = false;
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await _viewModel.HandleScannedCodeAsync(value);
            CameraView.IsDetecting = true;
        });
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _pendingCode = query.TryGetValue("code", out var code) ? code?.ToString() : null;
    }
}
