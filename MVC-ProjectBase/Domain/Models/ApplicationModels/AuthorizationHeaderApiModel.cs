namespace Domain.Models.ApplicationModels
{
    public class AuthorizationHeaderApiModel
    {
        public required string Type { get; set; }
        public required string Authorization { get; set; }
    }
}
