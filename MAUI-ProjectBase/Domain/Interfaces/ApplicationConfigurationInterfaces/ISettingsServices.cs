using Domain.Models.ApplicationConfigurationModels;

namespace Domain.Interfaces.ApplicationConfigurationInterfaces
{
    public interface ISettingsServices
    {
        AppLanguageModel _currentLanguage { get; }
        event Action? OnLanguageChanged;
        List<AppLanguageModel> AvailableLanguages();
        void ChangeLanguage(string languageCode);

        event Action? OnSkinChanged;
        List<AppSkinModel> AvailableSkins();
        void ChangeSkin(string skin);
    }
}
