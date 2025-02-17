namespace Domain.Models.ApplicationConfigurationModels
{
    public class AppSettingsModel
    {
        public string? AppName { get; set; }
        public double? AppVersion { get; set; }
        public string? Skin { get; set; }
        public string? Language { get; set; }
        public List<DataBaseConnectionModel>? DataBaseConnectionModels { get; set; }
        public List<ApiConnectionModel>? ApiConnections { get; set; }
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
        public required string Type { get; set; }
    }
}
