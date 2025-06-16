using Domain.Models.ApplicationConfigurationModels;

namespace Domain.Interfaces.ApplicationConfigurationInterfaces
{
    public interface ISettingsServices
    {
        AppLanguageModel currentLanguage { get; }
        event Action? OnLanguageChanged;
        List<AppLanguageModel> AvailableLanguages();
        void ChangeLanguage(string languageCode);

        AppThemeModel currentTheme { get; }
        event Action? OnThemeChanged;
        List<AppThemeModel> AvailableThemes();
        void ChangeTheme(string theme);
    }
}
