using Newtonsoft.Json.Linq;

namespace Domain.Models.ApplicationModels
{
    public class GraphQlApiResponseModel
    {
        public int? StatusCode { get; set; }
        public JObject? data { get; set; }
        public List<GraphQlApiErrorResponseModel>? errors { get; set; }
    }

    public class GraphQlApiErrorResponseModel
    {
        public string? message { get; set; }
        public List<GraphQlApiErrorLocationsResponseModel>? locations { get; set; }
        public GraphQlApiErrorExtensionsResponseModel? extensions { get; set; }
    }

    public class GraphQlApiErrorExtensionsResponseModel
    {
        public string? code { get; set; }
        public List<string?>? codes { get; set; }
        public string? number { get; set; }
    }

    public class GraphQlApiErrorLocationsResponseModel
    {
        public int? column { get; set; }
        public int? line { get; set; }
    }
}
