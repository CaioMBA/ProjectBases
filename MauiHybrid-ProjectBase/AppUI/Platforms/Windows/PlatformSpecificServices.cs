using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Domain.Interfaces.ApplicationConfigurationInterfaces;
using Microsoft.Win32;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;
using Windows.Storage.Pickers;
using WinRT.Interop;

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
            var assetFiles = new ConcurrentBag<string>();
            var assetsPath = Package.Current.InstalledLocation.Path;

            if (Directory.Exists(assetsPath))
            {
                GetFilesRecursiveParallel(assetsPath, assetFiles);
            }

            return await Task.FromResult(assetFiles);
        }

        private static void GetFilesRecursiveParallel(string directory, ConcurrentBag<string> fileList)
        {
            try
            {
                string basePath = Package.Current.InstalledLocation.Path;
                string relativeDirectory = directory.Replace($"{basePath}{Path.DirectorySeparatorChar}", "");
                if (!Directory.Exists(directory))
                {
                    return;
                }
                if (relativeDirectory.StartsWith(@"wwwroot\lib", StringComparison.OrdinalIgnoreCase)
                    || relativeDirectory.StartsWith(@"wwwroot/lib", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }


                foreach (var file in Directory.EnumerateFiles(directory))
                {
                    var relativePath = file.Replace($"{basePath}{Path.DirectorySeparatorChar}", "");
                    fileList.Add(relativePath);
                }

                Parallel.ForEach(Directory.EnumerateDirectories(directory), subDir =>
                {
                    GetFilesRecursiveParallel(subDir, fileList);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on AppUI.Platforms.Windows > GetFilesRecursiveParallel. Error: {ex.Message}");
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

        #region SystemInfo
        public long GetStorage(bool available = false, string? name = null)
        {
            return DriveInfo.GetDrives()
                .Where(d => d.IsReady
                            && d.DriveType == DriveType.Fixed
                            && (String.IsNullOrWhiteSpace(name) || d.Name == name))
                .Sum(d => (available ? d.TotalFreeSpace : d.TotalSize));
        }

        public string GetProcessor()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");
                return key?.GetValue("ProcessorNameString")?.ToString() ?? "Unknown Processor";
            }
            catch
            {
                return "Unknown Processor";
            }
        }

        public long GetRam()
        {
            return GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
        }

        public string GetGraphicsCard()
        {
            try
            {
                string tempDxDiagPath = Path.Combine(Path.GetTempPath(), "dxdiag_output.txt");

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "dxdiag",
                        Arguments = $"/t \"{tempDxDiagPath}\"",
                        RedirectStandardOutput = false,
                        Verb = "runas", // Run as administrator
                        UseShellExecute = true,
                        CreateNoWindow = true,

                    }
                };

                process.Start();
                process.WaitForExit(5000);

                if (File.Exists(tempDxDiagPath))
                {
                    var lines = File.ReadAllLines(tempDxDiagPath);

                    // Look for the first "Card name" line
                    string? cardname = lines.FirstOrDefault(line => line.TrimStart().StartsWith("Card name:", StringComparison.OrdinalIgnoreCase))?.Split(':')[1].Trim();

                    File.Delete(tempDxDiagPath);

                    if (!String.IsNullOrWhiteSpace(cardname))
                    {
                        return cardname;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving GPU info via dxdiag: {ex.Message}");
            }

            return "Unknown Graphics Card";
        }

        public string GetOsName()
        {
            return RuntimeInformation.OSDescription switch
            {
                string desc when desc.Contains("Windows") => "Windows",
                string desc when desc.Contains("Linux") => "Linux",
                string desc when desc.Contains("Mac OS") => "macOS",
                _ => "Unknown OS"
            };
        }

        public string GetOsVersion() => Environment.OSVersion.Version.Major.ToString();

        public string GetOsArchitecture()
        {
            return RuntimeInformation.OSArchitecture switch
            {
                Architecture.X64 => "x64",
                Architecture.Arm64 => "arm64",
                Architecture.X86 => "x86",
                _ => "Unknown Architecture"
            };
        }

        public string GetMachineName() => Environment.MachineName;

        public string GetUserName() => Environment.UserName;
        #endregion
    }
}
