namespace Domain.Models.ApplicationModels
{
    public class RestApiRequestModel
    {
        public required string Url { get; set; }
        public required string TypeRequest { get; set; }
        public string? Body { get; set; }
        public double? TimeOut { get; set; }
        public Dictionary<string, string?>? Headers { get; set; }
        public AuthorizationHeaderApiModel? Auth { get; set; }
    }
}
