using Domain.Interfaces.ApplicationConfiguration;
using Foundation;

[assembly: Dependency(typeof(AppUI.Platforms.iOS.AssetService))]
namespace AppUI.Platforms.iOS
{
    public class AssetService : IAssetService
    {
        public async Task<IEnumerable<string>> ListAssetsAsync()
        {
            List<string> assetFiles = new();
            var bundlePath = NSBundle.MainBundle.BundlePath;
            var resourcePath = System.IO.Path.Combine(bundlePath, "Resources", "Raw");

            if (System.IO.Directory.Exists(resourcePath))
            {
                GetFilesRecursive(resourcePath, assetFiles);
            }

            return await Task.FromResult(assetFiles);
        }

        private void GetFilesRecursive(string directory, List<string> fileList)
        {
            try
            {
                var files = System.IO.Directory.GetFiles(directory);
                foreach (var file in files)
                {
                    var relativePath = file.Replace($"{NSBundle.MainBundle.BundlePath}/", "");
                    fileList.Add(relativePath);
                }

                var subDirectories = System.IO.Directory.GetDirectories(directory);
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
