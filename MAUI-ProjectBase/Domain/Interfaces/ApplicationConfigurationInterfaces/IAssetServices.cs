namespace Domain.Interfaces.ApplicationConfigurationInterfaces
{
    public interface IAssetServices
    {
        Task<IEnumerable<string>> ListAssetsAsync();
        string ReadAssetContent(string path);
    }
}
