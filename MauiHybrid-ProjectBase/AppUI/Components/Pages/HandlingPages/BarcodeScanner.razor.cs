using Microsoft.Maui.Controls.Shapes;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace AppUI.Components.Pages.HandlingPages;

public class BarcodeScanner : ContentPage
{
    private readonly TaskCompletionSource<string?> _scanResultSource = new();
    private readonly CameraBarcodeReaderView _scanner;
    private bool _isClosing = false;

    public BarcodeScanner()
    {
        BackgroundColor = Colors.Transparent;

        _scanner = new CameraBarcodeReaderView
        {
            IsDetecting = true,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Options = new BarcodeReaderOptions
            {
                AutoRotate = true,
                TryHarder = false,
                TryInverted = false,
                Multiple = false,
                Formats = BarcodeFormat.QrCode 
                | BarcodeFormat.Ean13 
                | BarcodeFormat.Code128
            }
        };

        _scanner.BarcodesDetected += Scanner_BarcodesDetected;

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
                    Command = new Command(() => _scanner.IsTorchOn = !_scanner.IsTorchOn)
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

    private async void Scanner_BarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
    {
        var result = e.Results.FirstOrDefault()?.Value;
        if (!string.IsNullOrEmpty(result))
        {
            await MainThread.InvokeOnMainThreadAsync(() => CloseScannerAsync(result));
        }
    }

    private async Task CloseScannerAsync(string? result)
    {
        if (_isClosing) return;
        _isClosing = true;

        // Cleanup
        _scanner.IsDetecting = false;
        _scanner.BarcodesDetected -= Scanner_BarcodesDetected;
        _scanResultSource.TrySetResult(result);

        var nav = Application.Current?.Windows.FirstOrDefault()?.Page?.Navigation;
        if (nav != null)
        {
            await nav.PopModalAsync();
        }
    }

    public Task<string?> GetResultAsync() => _scanResultSource.Task;
}
