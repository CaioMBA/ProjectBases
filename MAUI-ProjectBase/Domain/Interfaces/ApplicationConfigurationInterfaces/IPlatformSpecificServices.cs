namespace Domain.Interfaces.ApplicationConfigurationInterfaces
{
    public interface IPlatformSpecificServices
    {
        string ReadAssetContent(string path);

        Task<IEnumerable<string>> ListAssetsAsync();

        Task<string?> PickDirectory();

        Task OpenDirectory(string folderPath);
    }
}
