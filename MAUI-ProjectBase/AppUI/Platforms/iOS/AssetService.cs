using Domain.Interfaces.ApplicationConfigurationInterfaces;
using Foundation;

[assembly: Dependency(typeof(AppUI.Platforms.iOS.AssetService))]
namespace AppUI.Platforms.iOS
{
    public class AssetService : IAssetServices
    {
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
    }
}
