namespace Domain.Models.ApplicationConfigurationModels.ApiDefaultModels
{
    public class RestApiRequestModel
    {
        public required string Url { get; set; }
        public required string TypeRequest { get; set; }
        public string? Body { get; set; }
        public double? TimeOut { get; set; }
        public Dictionary<string, string?>? Headers { get; set; }
        public Dictionary<string, string?>? QueryParameters { get; set; }
        public AuthenticationHeaderModel? Authentication { get; set; }
    }
}
