using Domain.Interfaces.ApplicationConfiguration;
using Windows.ApplicationModel;

[assembly: Dependency(typeof(AppUI.Platforms.Windows.AssetService))]
namespace AppUI.Platforms.Windows
{
    public class AssetService : IAssetService
    {
        public async Task<IEnumerable<string>> ListAssetsAsync()
        {
            var assetFiles = new List<string>();
            var assetsPath = Path.Combine(Package.Current.InstalledLocation.Path, "Resources", "Raw");

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
    }
}
