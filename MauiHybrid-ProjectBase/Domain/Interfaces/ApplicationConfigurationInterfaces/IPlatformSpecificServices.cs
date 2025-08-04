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
        long GetStorage(bool available = false, string? name = null);
        string GetProcessor();
        long GetRam();
        string GetGraphicsCard();
        string GetOsName();
        string GetOsVersion();
        string GetOsArchitecture();
        string GetMachineName();
        string GetUserName();
    }
}
