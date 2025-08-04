using CommunityToolkit.Maui.Storage;
using Domain.Interfaces.ApplicationConfigurationInterfaces;
using Foundation;
using MobileCoreServices;
using ObjCRuntime;
using Plugin.LocalNotification;
using Plugin.LocalNotification.iOSOption;
using UIKit;

[assembly: Dependency(typeof(AppUI.Platforms.iOS.PlatformSpecificServices))]
namespace AppUI.Platforms.iOS
{
    public class PlatformSpecificServices : IPlatformSpecificServices
    {
        #region Assets
        public string ReadAssetContent(string path)
        {
            string content = string.Empty;
            var bundlePath = NSBundle.MainBundle.BundlePath;
            try
            {
                var fullPath = $"{bundlePath}/{path}";
                if (File.Exists(fullPath))
                {
                    content = File.ReadAllText(fullPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on AppUI.Platforms.iOS > ReadAssetContent. Error: {ex.Message}");
            }
            return content;
        }

        public async Task<IEnumerable<string>> ListAssetsAsync()
        {
            List<string> assetFiles = new();
            var bundlePath = NSBundle.MainBundle.BundlePath;

            if (Directory.Exists(bundlePath))
            {
                GetFilesRecursive(bundlePath, assetFiles);
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
                    var relativePath = file.Replace($"{NSBundle.MainBundle.BundlePath}/", "");
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
                Console.WriteLine($"Error on AppUI.Platforms.iOS > GetFilesRecursive. Error: {ex.Message}");
            }
        }
        #endregion

        #region Picker
        public async Task<string?> PickDirectory()
        {
            try
            {
                FolderPickerResult? folder = await FolderPicker.PickAsync(default);
                if (folder == null)
                {
                    return null;
                }
                if (folder.IsSuccessful)
                {
                    return folder.Folder.Path;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on AppUI.Platforms.iOS > PickDirectory. Error: {ex.Message}");
                return null;
            }
        }

        public async Task OpenDirectory(string folderPath)
        {
            try
            {
                Console.WriteLine($"IOS DOESN'T ALLOW TO OPEN SPECIFIC FOLDER IN FILES APP");
                var picker = new UIDocumentPickerViewController(new string[] { UTType.Folder }, UIDocumentPickerMode.Open);
                picker.WasCancelled += (sender, e) => { Console.WriteLine("User canceled folder selection"); };

                var window = UIApplication.SharedApplication.KeyWindow;
                var viewController = window.RootViewController;
                viewController.PresentViewController(picker, true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on AppUI.Platforms.iOS > OpenDirectory. Error: {ex.Message}");
            }
        }
        #endregion

        #region Local Notifications
        public async Task SendLocalNotification(string title, string message, double NotifyTime = 1)
        {
            var notification = new NotificationRequest
            {
                NotificationId = new Random().Next(int.MinValue, int.MaxValue),
                Title = title,
                Description = message,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(NotifyTime)
                },
                iOS = new iOSOptions
                {
                    Priority = iOSPriority.Critical,
                    ApplyBadgeValue = true,
                    PresentAsBanner = true,
                    ShowInNotificationCenter = true
                }
            };

            await LocalNotificationCenter.Current.Show(notification);
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
            // The 'name' parameter is ignored on iOS as it typically has a single main storage volume.
            try
            {
                NSFileSystemAttributes? attributes = NSFileManager.DefaultManager.GetFileSystemAttributes(
                    Environment.GetFolderPath(Environment.SpecialFolder.Personal)
                );

                long? storage = available ? (long?)attributes?.FreeSize : (long?)attributes?.Size;
                if (storage == null)
                {
                    Console.WriteLine("Failed to retrieve storage information.");
                    return 0;
                }

                return (long)storage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetStorage Error] Failed to retrieve storage information: {ex}");
                return 0;
            }
        }

        public string GetProcessor()
        {
            return "Apple A-Series"; // No public API to get actual chip (A14, A15, etc.)
        }

        public long GetRam()
        {
            return (long)NSProcessInfo.ProcessInfo.PhysicalMemory;
        }

        public string GetGraphicsCard()
        {
            return "Apple GPU"; // No public API for actual GPU
        }

        public string GetOsName() => "iOS";

        public string GetOsVersion() => UIDevice.CurrentDevice.SystemVersion;

        public string GetOsArchitecture()
        {
            var arch = Runtime.Arch;
            return arch.ToString();
        }

        public string GetMachineName()
        {
            return UIDevice.CurrentDevice.Name;
        }

        public string GetUserName()
        {
            return UIDevice.CurrentDevice.IdentifierForVendor?.AsString() ?? "Unknown User";
        }
        #endregion
    }
}
