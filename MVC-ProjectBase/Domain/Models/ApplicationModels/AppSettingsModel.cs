namespace Domain.Models.ApplicationModels
{
    public class AppSettingsModel
    {
        public string? AppName { get; set; }
        public double? AppVersion { get; set; }
        public string? Skin { get; set; } = "light";
        public List<DataBaseConnectionModel>? DataBaseConnections { get; set; }
        public List<ApiConnectionModel>? ApiConnections { get; set; }

    }

    public class DataBaseConnectionModel
    {
        public required string Name { get; set; }
        public required string Type { get; set; }
        public required string ConnectionString { get; set; }
    }
    public class ApiConnectionModel
    {
        public required string Name { get; set; }
        public required string Type { get; set; }
        public required string Method { get; set; }
        public required string Url { get; set; }
    }
}
