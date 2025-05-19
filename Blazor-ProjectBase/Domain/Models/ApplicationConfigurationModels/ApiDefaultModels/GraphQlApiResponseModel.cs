using Newtonsoft.Json.Linq;

namespace Domain.Models.ApplicationConfigurationModels.ApiDefaultModels
{
    public record GraphQlApiResponseModel
    {
        public int? StatusCode { get; set; }
        public JObject? Data { get; set; }
        public List<GraphQlApiErrorResponseModel>? Errors { get; set; }
    }

    public record GraphQlApiErrorResponseModel
    {
        public string? Message { get; set; }
        public List<GraphQlApiErrorLocationsResponseModel>? Locations { get; set; }
        public GraphQlApiErrorExtensionsResponseModel? Extensions { get; set; }
    }

    public record GraphQlApiErrorExtensionsResponseModel
    {
        public string? Code { get; set; }
        public List<string?>? Codes { get; set; }
        public string? Number { get; set; }
    }

    public record GraphQlApiErrorLocationsResponseModel
    {
        public int? Column { get; set; }
        public int? Line { get; set; }
    }
}
