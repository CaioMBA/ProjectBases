using Domain.Models.ApplicationConfigurationModels;

namespace Domain.Interfaces.ApplicationConfigurationInterfaces
{
    public interface ISettingsServices
    {
        AppLanguageModel _currentLanguage { get; }

        event Action? OnLanguageChanged;
        event Action? OnSkinChanged;

        void ChangeLanguage(string languageCode);
        void ChangeSkin(string skin);
    }
}
