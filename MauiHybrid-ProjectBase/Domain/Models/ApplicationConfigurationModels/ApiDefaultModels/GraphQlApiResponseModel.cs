using Newtonsoft.Json.Linq;

namespace Domain.Models.ApplicationConfigurationModels.ApiDefaultModels
{
    public class GraphQlApiResponseModel
    {
        public int? StatusCode { get; set; }
        public JObject? Data { get; set; }
        public List<GraphQlApiErrorResponseModel>? Errors { get; set; }
    }

    public class GraphQlApiErrorResponseModel
    {
        public string? Message { get; set; }
        public List<GraphQlApiErrorLocationsResponseModel>? Locations { get; set; }
        public GraphQlApiErrorExtensionsResponseModel? Extensions { get; set; }
    }

    public class GraphQlApiErrorExtensionsResponseModel
    {
        public string? Code { get; set; }
        public List<string?>? Codes { get; set; }
        public string? Number { get; set; }
    }

    public class GraphQlApiErrorLocationsResponseModel
    {
        public int? Column { get; set; }
        public int? Line { get; set; }
    }
}
