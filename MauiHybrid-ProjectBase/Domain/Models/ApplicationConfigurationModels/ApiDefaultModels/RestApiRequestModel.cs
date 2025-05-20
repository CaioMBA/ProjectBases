using Domain.Enums;

namespace Domain.Models.ApplicationConfigurationModels.ApiDefaultModels
{
    public class RestApiRequestModel
    {
        public required string Url { get; set; }
        public required ApiRequestMethod TypeRequest { get; set; }
        public string? Body { get; set; }
        public double? TimeOut { get; set; }
        public IDictionary<string, string?>? Headers { get; set; }
        public IDictionary<string, string?>? QueryParameters { get; set; }
        public AuthenticationHeaderModel? Authentication { get; set; }
    }
}
