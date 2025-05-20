namespace Domain.Interfaces.ApplicationConfigurationInterfaces
{
    public interface IPlatformSpecificServices
    {
        string ReadAssetContent(string path);

        Task<IEnumerable<string>> ListAssetsAsync();

        Task<string?> PickDirectory();

        Task OpenDirectory(string folderPath);

        Task SendLocalNotification(string title, string message, double NotifyTime = 1);

        Task<string?> ScanBarcodeAsync();
    }
}
