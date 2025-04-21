using Domain.Enums;

namespace Domain.Models.ApplicationConfigurationModels
{
    public record AppSettingsModel
    {
        public string? AppName { get; set; }
        public double? AppVersion { get; set; }
        public List<DataBaseConnectionModel>? DataBaseConnectionModels { get; set; }
        public List<ApiConnectionModel>? ApiConnections { get; set; }
    }

    public record DataBaseConnectionModel
    {
        public required string DataBaseID { get; set; }
        public required DataBaseType Type { get; set; }
        public required string ConnectionString { get; set; }
        public string? Name { get; set; }
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
