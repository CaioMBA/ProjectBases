namespace Domain.Models.ApplicationConfigurationModels.ApiDefaultModels
{
    public record GraphQlApiRequestModel
    {
        public required string Url { get; set; }
        public required string Query { get; set; }
        public IDictionary<string, object?>? Variables { get; set; }
        public AuthenticationHeaderModel? Authentication { get; set; }
    }
}
