namespace Domain.Models.ApplicationConfigurationModels.ApiDefaultModels
{
    public class AuthenticationHeaderModel
    {
        public required string Type { get; set; }
        public required string Authorization { get; set; }
    }
}
