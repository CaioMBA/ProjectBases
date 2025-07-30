using Camera.MAUI;
using Camera.MAUI.ZXing;
using Camera.MAUI.ZXingHelper;
using Microsoft.Maui.Controls.Shapes;

namespace AppUI.Components.Pages.HandlingPages;

public class BarcodeScanner : ContentPage
{
    private readonly TaskCompletionSource<string?> _scanResultSource = new();
    private readonly CameraView _scanner;
    private bool _isClosing = false;

    public BarcodeScanner()
    {
        BackgroundColor = Colors.Transparent;

        _scanner = new CameraView
        {
            IsEnabled = true,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            FlashMode = FlashMode.Auto,
            ControlBarcodeResultDuplicate = true,
            BarCodeDetectionFrameRate = 10,
            BarCodeDetectionMaxThreads = 5,
            BarCodeDecoder = new ZXingBarcodeDecoder(),
            BarCodeDetectionEnabled = true,
            BarCodeOptions = new BarcodeDecodeOptions
            {
                PossibleFormats = [BarcodeFormat.QR_CODE, BarcodeFormat.CODE_128, BarcodeFormat.EAN_13],
                AutoRotate = true,
                ReadMultipleCodes = false,
                TryHarder = false,
                TryInverted = false
            },
        };

        _scanner.CamerasLoaded += CamerasLoaded;
        _scanner.BarcodeDetected += Scanner_BarcodesDetected;

        var header = new Grid
        {
            HeightRequest = 50,
            BackgroundColor = Colors.Black,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Children =
            {
                new Label
                {
                    Text = "Scan a Qr|Bar code",
                    TextColor = Colors.White,
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            }
        };

        var buttons = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Center,
            Spacing = 20,
            Margin = new Thickness(0, 10, 0, 10),
            Children =
            {
                new Button
                {
                    Text = "🔦 Flash",
                    BackgroundColor = Colors.DarkGoldenrod,
                    TextColor = Colors.White,
                    FontAttributes = FontAttributes.Bold,
                    CornerRadius = 5,
                    Command = new Command(() => _scanner.TorchEnabled = !_scanner.TorchEnabled)
                },
                new Button
                {
                    Text = "🛑 Cancel",
                    BackgroundColor = Colors.DarkRed,
                    TextColor = Colors.White,
                    FontAttributes = FontAttributes.Bold,
                    CornerRadius = 5,
                    Command = new Command(async () => await CloseScannerAsync(null))
                }
            }
        };

        var cameraContainer = new Grid
        {
            BackgroundColor = Colors.Gray,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Children = { _scanner }
        };

        var popupContent = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                new RowDefinition { Height = GridLength.Auto }
            },
            BackgroundColor = Colors.Black,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        Grid.SetRow(header, 0);
        Grid.SetRow(cameraContainer, 1);
        Grid.SetRow(buttons, 2);

        popupContent.Children.Add(header);
        popupContent.Children.Add(cameraContainer);
        popupContent.Children.Add(buttons);

        var outerWrapper = new Border
        {
            Stroke = Colors.Black,
            StrokeThickness = 2,
            BackgroundColor = Colors.Black,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(20)
            },
            Content = popupContent
        };

        Content = new Grid
        {
            Children = { outerWrapper }
        };

        this.SizeChanged += (_, _) =>
        {
            var width = this.Width * 0.95;
            var height = this.Height * 0.8;

            outerWrapper.WidthRequest = width * 1.01;
            outerWrapper.HeightRequest = height;

            popupContent.WidthRequest = width;
            popupContent.HeightRequest = height * 0.95;

            cameraContainer.WidthRequest = width;
            cameraContainer.HeightRequest = height * 0.6;

            _scanner.WidthRequest = width;
            _scanner.HeightRequest = height * 0.6;
        };
    }

    private async void CamerasLoaded(object? sender, EventArgs e)
    {
        if (_scanner?.NumMicrophonesDetected > 0)
        {
            _scanner.Microphone = _scanner.Microphones.FirstOrDefault();
        }
        if (_scanner?.NumCamerasDetected > 0)
        {
            _scanner.Camera = _scanner.Cameras.FirstOrDefault();
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    await _scanner.StopCameraAsync();
                }
                finally
                {
                    await _scanner.StartCameraAsync();
                }
            });
        }

    }

    private async void Scanner_BarcodesDetected(object? sender, BarcodeEventArgs e)
    {
        var result = e.Result.Select(x => new
        {
            x.Text,
            x.BarcodeFormat,
            x.RawBytes
        });

        if (result.Any())
        {
            await MainThread.InvokeOnMainThreadAsync(async () => { await CloseScannerAsync(result.FirstOrDefault()?.Text); });
        }
    }

    private async Task CloseScannerAsync(string? result)
    {
        if (_isClosing) return;
        _isClosing = true;

        // Cleanup
        try
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await _scanner.StopCameraAsync();

                var nav = Application.Current?.Windows.FirstOrDefault()?.Page?.Navigation;
                if (nav != null)
                {
                    await nav.PopModalAsync();
                }

                _scanResultSource.TrySetResult(result);
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error closing scanner: {ex.Message}");
        }
        finally
        {
            _isClosing = false;
        }

    }

    public Task<string?> GetResultAsync() => _scanResultSource.Task;

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _ = CloseScannerAsync(null);
    }
}
