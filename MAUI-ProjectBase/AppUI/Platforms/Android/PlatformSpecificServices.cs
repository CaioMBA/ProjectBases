using Android.Content;
using Android.Content.Res;
using CommunityToolkit.Maui.Storage;
using Domain.Interfaces.ApplicationConfigurationInterfaces;

[assembly: Dependency(typeof(AppUI.Platforms.Android.PlatformSpecificServices))]
namespace AppUI.Platforms.Android
{
    public class PlatformSpecificServices : IPlatformSpecificServices
    {
        #region Assets
        public string ReadAssetContent(string path)
        {
            string content = string.Empty;
            AssetManager? assets = Platform.AppContext.Assets;
            try
            {
                using Stream? stream = assets.Open(path);
                using StreamReader? reader = new(stream);
                content = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on AppUI.Platforms.Android > ReadAssetContent. Error: {ex.Message}");
            }
            return content;
        }

        public async Task<IEnumerable<string>> ListAssetsAsync()
        {
            List<string> assetFiles = new();
            AssetManager? assets = Platform.AppContext.Assets;

            ListAssetsRecursive(assets, "", assetFiles);

            return await Task.FromResult(assetFiles);
        }

        private void ListAssetsRecursive(AssetManager assets, string path, List<string> fileList)
        {
            try
            {
                string[] files = assets.List(path);
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        string fullPath = string.IsNullOrEmpty(path) ? file : $"{path}/{file}";

                        string[] subFiles = assets.List(fullPath);
                        if (subFiles?.Length > 0)
                        {
                            ListAssetsRecursive(assets, fullPath, fileList);
                        }
                        else
                        {
                            fileList.Add(fullPath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on AppUI.Platforms.Android > ListAssetsRecursive. Error: {ex.Message}");
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
                Console.WriteLine($"Error on AppUI.Platforms.Android > PickDirectory. Error: {ex.Message}");
                return null;
            }
        }

        public async Task OpenDirectory(string folderPath)
        {
            try
            {
                var file = new Java.IO.File(folderPath);
                var uri = FileProvider.GetUriForFile(Platform.CurrentActivity, $"{Platform.CurrentActivity.PackageName}.fileprovider", file);

                var intent = new Intent(Intent.ActionView);
                intent.SetDataAndType(uri, "*/*");
                intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.GrantReadUriPermission);

                Platform.CurrentActivity.StartActivity(intent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on AppUI.Platforms.Android > OpenDirectory. Error: {ex.Message}");
            }
        }
        #endregion
    }
}
