using Domain.Enums;

namespace Domain.Models.ApplicationConfigurationModels.ApiDefaultModels
{
    public record AuthenticationHeaderModel
    {
        public required ApiAuthorizationType Type { get; set; }
        public required string Authorization { get; set; }
    }
}
