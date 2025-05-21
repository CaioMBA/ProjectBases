using Microsoft.Maui.Controls.Shapes;
using ZXing.Net.Maui.Controls;

namespace AppUI.Components.Pages.HandlingPages;

public class BarcodeScanner : ContentPage
{
    private readonly TaskCompletionSource<string?> _scanResultSource = new();
    private readonly CameraBarcodeReaderView _scanner;

    public BarcodeScanner()
    {
        BackgroundColor = Color.FromRgba(0, 0, 0, 0.75); // Dark semi-transparent overlay

        _scanner = new CameraBarcodeReaderView
        {
            IsDetecting = true,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        _scanner.BarcodesDetected += (s, e) =>
        {
            var result = e.Results.FirstOrDefault()?.Value;
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                _scanResultSource.TrySetResult(result);
                var nav = Application.Current?.Windows.FirstOrDefault()?.Page?.Navigation;
                if (nav != null)
                {
                    await nav.PopModalAsync();
                }
            });
        };

        var scannerContainer = new Grid
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Children = { _scanner }
        };

        var scannerFrame = new Border
        {
            Stroke = Colors.White,
            StrokeThickness = 2,
            Background = new SolidColorBrush(Colors.Black),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = 20
            },
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Content = scannerContainer
        };

        var flashlightButton = new Button
        {
            Text = "🔦 Flash",
            BackgroundColor = Colors.Transparent,
            TextColor = Colors.White,
            FontSize = 16
        };

        flashlightButton.Clicked += (s, e) =>
        {
            _scanner.IsTorchOn = !_scanner.IsTorchOn;
        };

        var cancelButton = new Button
        {
            Text = "Cancel",
            BackgroundColor = Colors.Transparent,
            TextColor = Colors.Red,
            FontSize = 16
        };

        cancelButton.Clicked += async (s, e) =>
        {
            _scanResultSource.TrySetResult(null);
            var nav = Application.Current?.Windows.FirstOrDefault()?.Page?.Navigation;
            if (nav != null)
            {
                await nav.PopModalAsync();
            }
        };

        var buttonStack = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Center,
            Spacing = 20,
            Children = { flashlightButton, cancelButton }
        };

        var popupCard = new Border
        {
            Stroke = Colors.White,
            StrokeThickness = 2,
            Background = new SolidColorBrush(Color.FromArgb("#1E1E1E")),
            Shadow = new Shadow
            {
                Brush = Brush.Black,
                Offset = new Point(4, 4),
                Radius = 8,
                Opacity = 0.5f
            },
            StrokeShape = new RoundRectangle
            {
                CornerRadius = 25
            },
            Margin = 30,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Content = new StackLayout
            {
                Spacing = 20,
                Padding = 20,
                Children =
                {
                    new Label
                    {
                        Text = "Scan a Barcode",
                        TextColor = Colors.White,
                        FontSize = 18,
                        HorizontalOptions = LayoutOptions.Center
                    },
                    scannerFrame,
                    buttonStack
                }
            }
        };


        Content = new Grid
        {
            Children = { popupCard }
        };


        this.SizeChanged += (_, _) =>
        {
            var viewWidth = this.Width;
            var viewHeight = this.Height;

            string platform = DeviceInfo.Platform.ToString();

            double scannerWidth = platform switch
            {
                "Android" => viewWidth * 0.8,
                "WinUI" => viewWidth * 0.5,
                "MacCatalyst" => viewWidth * 0.55,
                _ => viewWidth * 0.6
            };
            double scannerHeight = platform switch
            {
                "Android" => viewHeight * 0.4,
                "WinUI" => viewHeight * 0.45,
                "MacCatalyst" => viewHeight * 0.5,
                _ => viewHeight * 0.5
            };

            _scanner.WidthRequest = scannerWidth;
            _scanner.HeightRequest = scannerHeight;

            scannerContainer.WidthRequest = scannerWidth;
            scannerContainer.HeightRequest = scannerHeight;
            scannerContainer.Clip = new RoundRectangleGeometry
            {
                CornerRadius = new CornerRadius(20),
                Rect = new Rect(0, 0, scannerWidth, scannerHeight)
            };
        };
    }

    public Task<string?> GetResultAsync() => _scanResultSource.Task;
}
