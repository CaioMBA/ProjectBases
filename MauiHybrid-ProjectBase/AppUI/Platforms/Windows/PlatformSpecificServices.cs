﻿using Domain.Interfaces.ApplicationConfigurationInterfaces;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;
using Windows.Storage.Pickers;
using WinRT.Interop;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Alerts;

[assembly: Dependency(typeof(AppUI.Platforms.Windows.PlatformSpecificServices))]
namespace AppUI.Platforms.Windows
{
    public class PlatformSpecificServices : IPlatformSpecificServices
    {
        #region Assets
        public string ReadAssetContent(string path)
        {
            var content = string.Empty;
            var assetsPath = Package.Current.InstalledLocation.Path;
            try
            {
                var fullPath = Path.Combine(assetsPath, path);
                if (File.Exists(fullPath))
                {
                    content = File.ReadAllText(fullPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on AppUI.Platforms.Windows > ReadAssetContent. Error: {ex.Message}");
            }
            return content;
        }

        public async Task<IEnumerable<string>> ListAssetsAsync()
        {
            var assetFiles = new List<string>();
            var assetsPath = Package.Current.InstalledLocation.Path;

            if (Directory.Exists(assetsPath))
            {
                GetFilesRecursive(assetsPath, assetFiles);
            }

            return await Task.FromResult(assetFiles);
        }

        private void GetFilesRecursive(string directory, List<string> fileList)
        {
            try
            {
                var files = Directory.GetFiles(directory);
                foreach (var file in files)
                {
                    var relativePath = file.Replace($"{Package.Current.InstalledLocation.Path}{Path.DirectorySeparatorChar}", "");
                    fileList.Add(relativePath);
                }

                var subDirectories = Directory.GetDirectories(directory);
                foreach (var subDir in subDirectories)
                {
                    GetFilesRecursive(subDir, fileList);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on AppUI.Platforms.Windows > GetFilesRecursive. Error: {ex.Message}");
            }
        }
        #endregion

        #region Picker
        public async Task<string?> PickDirectory()
        {
            var folderPicker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.Desktop,
                ViewMode = PickerViewMode.List
            };
            folderPicker.FileTypeFilter.Add("*");

            var hwnd = GetWindowHandle();
            if (hwnd == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to get a valid window handle.");
            }

            InitializeWithWindow.Initialize(folderPicker, hwnd);

            var folder = await folderPicker.PickSingleFolderAsync();
            return folder?.Path;
        }

        private static IntPtr GetWindowHandle()
        {
            var mauiWindow = Application.Current?.Windows.FirstOrDefault();
            if (mauiWindow?.Handler?.PlatformView is Microsoft.UI.Xaml.Window nativeWindow)
            {
                return WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            }

            return IntPtr.Zero;
        }

        public async Task OpenDirectory(string folderPath)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start("explorer.exe", folderPath);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", folderPath);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", folderPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on AppUI.Platforms.Windows > OpenDirectory. Error: {ex.Message}");
            }
        }
        #endregion

        #region Local Notifications
        public async Task SendLocalNotification(string title, string message, double NotifyTime = 1)
        {
            var duration = NotifyTime <= 5 ? ToastDuration.Short : ToastDuration.Long;
            var msg = $"{title}: {message}";
            if (String.IsNullOrEmpty(title))
            {
                msg = message;
            }

            var toast = Toast.Make(msg, duration);
            await toast.Show();
        }
        #endregion

        #region Camera
        public async Task<string?> ScanBarcodeAsync()
        {
            var scannerPage = new AppUI.Components.Pages.HandlingPages.BarcodeScanner();
            var nav = Application.Current?.Windows.FirstOrDefault()?.Page?.Navigation;
            if (nav == null)
            {
                throw new InvalidOperationException("No navigation context available.");
            }
            await nav.PushModalAsync(scannerPage);
            return await scannerPage.GetResultAsync();
        }
        #endregion
    }
}
