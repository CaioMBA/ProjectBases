namespace Domain.Models.ApplicationConfigurationModels
{
    public class AppLanguageModel
    {
        public string? LanguageCode { get; set; }
        public string? LanguageFlag { get; set; }
        public string? not_found_p { get; set; }
        public string? not_found_a { get; set; }
        public string? not_authorized_p { get; set; }
        public string? not_authorized_a { get; set; }
        public string? login_username_input_label { get; set; }
        public string? login_password_input_label { get; set; }
    }
}
