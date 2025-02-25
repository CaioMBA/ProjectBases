using Domain.Models.ApplicationConfigurationModels;
using Domain;
using Domain.Interfaces.ApplicationConfigurationInterfaces;

namespace Services
{
    public class SettingsServices : ISettingsServices
    {
        private readonly AppUtils _utils;
        private readonly AppSettingsModel _settings;
        private readonly List<AppLanguageModel> _availableLanguages;
        private readonly List<AppSkinModel> _availableSkins;

        public event Action? OnLanguageChanged;
        public event Action? OnSkinChanged;

        public AppLanguageModel _currentLanguage { get; private set; }
        public AppSkinModel _currentSkin { get; private set; }

        public SettingsServices(AppUtils utils,
                                List<AppLanguageModel> availableLanguages,
                                List<AppSkinModel> availableSkins,
                                AppSettingsModel settings)
        {
            _settings = settings;
            _utils = utils;
            _availableLanguages = availableLanguages;
            _availableSkins = availableSkins;
            _currentLanguage = _availableLanguages.FirstOrDefault(x => x.LanguageCode == (_settings.Language ?? "en-us"))!;
            _currentSkin = _availableSkins.FirstOrDefault(x => x.Name == (_settings.Skin ?? "light"))!;
        }

        #region Language
        public List<AppLanguageModel> AvailableLanguages()
        {
            return _availableLanguages;
        }

        public void ChangeLanguage(string languageCode)
        {
            var newLanguage = _availableLanguages.FirstOrDefault(x => x.LanguageCode == languageCode);
            if (newLanguage != null && newLanguage != _currentLanguage)
            {
                _settings.Language = languageCode;
                _currentLanguage = newLanguage;
                OnLanguageChanged?.Invoke();
            }
        }
        #endregion Language


        #region Skin
        public List<AppSkinModel> AvailableSkins()
        {
            return _availableSkins;
        }

        public void ChangeSkin(string skin)
        {
            var newSkin = _availableSkins.FirstOrDefault(x => x.Name == skin);
            if (newSkin != null && newSkin != _currentSkin)
            {
                _settings.Skin = skin;
                _currentSkin = newSkin;
                OnSkinChanged?.Invoke();
            }
        }
        #endregion Skin
    }
}
