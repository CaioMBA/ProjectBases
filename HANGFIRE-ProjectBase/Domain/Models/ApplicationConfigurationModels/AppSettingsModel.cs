namespace Domain.Models.ApplicationConfigurationModels
{
    public class AppSettingsModel
    {
        public string? AppName { get; set; }
        public double? AppVersion { get; set; }
        public string? Skin { get; set; }
        public string? Language { get; set; }
        public HangFireSettingsModel? Hangfire { get; set; }
        public List<DataBaseConnectionModel>? DataBaseConnectionModels { get; set; }
        public List<ApiConnectionModel>? ApiConnections { get; set; }
    }
    public class HangFireSettingsModel
    {
        public int WorkerCount { get; set; }
        public HangFireSettingsAuthorizationModel? AuthorizationCredential { get; set; }
    }
    public class HangFireSettingsAuthorizationModel
    {
        public string? User { get; set; }
        public string? Password { get; set; }
    }
    public class DataBaseConnectionModel
    {
        public required string DataBaseID { get; set; }
        public required string Type { get; set; }
        public required string ConnectionString { get; set; }
        public string? Name { get; set; }
        public string? Collection { get; set; }
    }
    public class ApiConnectionModel
    {
        public required string ApiID { get; set; }
        public required string Url { get; set; }
        public required List<ApiEndPointConnectionModel>? EndPoints { get; set; }
    }
    public class ApiEndPointConnectionModel
    {
        public required string ApiEndPointID { get; set; }
        public required string Path { get; set; }
        public required string Method { get; set; }
        public required string Type { get; set; }
    }
}
