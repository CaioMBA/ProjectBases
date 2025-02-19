namespace Domain.Models.ApplicationConfigurationModels
{
    public class UserSessionModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public string? Language { get; set; }
        public string? Role { get; set; }
    }
}
