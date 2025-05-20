using CommunityToolkit.Maui.Storage;
using Domain.Interfaces.ApplicationConfigurationInterfaces;
using Foundation;
using MobileCoreServices;
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
    }
}
