using Microsoft.Maui.Controls.Shapes;
using ZXing.Net.Maui.Controls;

namespace AppUI.Components.Pages.HandlingPages
{
    public class BarcodeScanner : ContentPage
    {
        private readonly TaskCompletionSource<string?> _scanResultSource = new();
        private readonly CameraBarcodeReaderView _scanner;

        public BarcodeScanner()
        {
            BackgroundColor = Colors.Black;

            _scanner = new CameraBarcodeReaderView
            {
                IsDetecting = true
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

            var flashlightButton = new Button
            {
                Text = "🔦 Flash",
                BackgroundColor = Colors.Black,
                TextColor = Colors.White,
                Margin = new Thickness(10),
                HorizontalOptions = LayoutOptions.Center
            };

            flashlightButton.Clicked += (s, e) =>
            {
                _scanner.IsTorchOn = !_scanner.IsTorchOn;
            };

            var cancelButton = new Button
            {
                Text = "Cancel",
                BackgroundColor = Colors.Red,
                TextColor = Colors.White,
                Margin = new Thickness(10),
                HorizontalOptions = LayoutOptions.Center
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

            Content = new Grid
            {
                Children =
                {
                    _scanner,
                    new StackLayout
                    {
                        VerticalOptions = LayoutOptions.End,
                        HorizontalOptions = LayoutOptions.Center,
                        Margin = new Thickness(0, 0, 0, 30),
                        Children = { flashlightButton, cancelButton }
                    }
                }
            };
        }

        public Task<string?> GetResultAsync() => _scanResultSource.Task;
    }
}
