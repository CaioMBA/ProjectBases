using Domain.Enums;

namespace Domain.Models.ApplicationConfigurationModels
{
    public class AppSettingsModel
    {
        public string? AppName { get; set; }
        public double? AppVersion { get; set; }
        public string? Skin { get; set; }
        public string? Language { get; set; }
        public HangFireSettingsModel? Hangfire { get; set; }
        public List<DataBaseConnectionModel>? DataBaseConnections { get; set; }
        public List<ApiConnectionModel>? ApiConnections { get; set; }
    }
    public class HangFireSettingsModel
    {
        public string? Identifier { get; set; }
        public int WorkerCount { get; set; }
        public HangFireSettingsAuthorizationModel? AuthorizationCredential { get; set; }
    }
    public class HangFireSettingsAuthorizationModel
    {
        public string? User { get; set; }
        public string? Password { get; set; }
    }
    public record DataBaseConnectionModel
    {
        public required string DataBaseID { get; set; }
        public required DataBaseType Type { get; set; }
        public string? ConnectionString { get; set; }
    }
    public record ApiConnectionModel
    {
        public required string ApiID { get; set; }
        public required string Url { get; set; }
        public required List<ApiEndPointConnectionModel>? EndPoints { get; set; }
    }
    public record ApiEndPointConnectionModel
    {
        public required string EndPointID { get; set; }
        public required string Path { get; set; }
        public required ApiRequestMethod Method { get; set; }
        public required ApiProtocolType Protocol { get; set; }
    }
}
