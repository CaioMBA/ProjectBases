namespace Domain.Interfaces.ApplicationConfiguration
{
    public interface IAssetService
    {
        Task<IEnumerable<string>> ListAssetsAsync();
    }
}
