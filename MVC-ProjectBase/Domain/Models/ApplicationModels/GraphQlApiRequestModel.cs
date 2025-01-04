namespace Domain.Models.ApplicationModels
{
    public class GraphQlApiRequestModel
    {
        public required string Url { get; set; }
        public required string Query { get; set; }
        public Dictionary<string, object?>? Variables { get; set; }
        public AuthorizationHeaderApiModel? Auth { get; set; }
    }
}
