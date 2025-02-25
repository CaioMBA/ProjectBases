using Domain.Models.ApplicationConfigurationModels;
using Domain;
using Domain.Interfaces.ApplicationConfigurationInterfaces;
using System.Globalization;

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
            _currentLanguage = GetStartLanguage();
            _currentSkin = GetStartSkin();
        }

        #region Language
        private AppLanguageModel GetStartLanguage()
        {
            return _availableLanguages.FirstOrDefault(x =>
                    x.LanguageCode!.Equals(CultureInfo.CurrentCulture.Name ?? _settings.Language, StringComparison.OrdinalIgnoreCase)
                ) ?? _availableLanguages.First();
        }

        public List<AppLanguageModel> AvailableLanguages()
        {
            return _availableLanguages;
        }

        public void ChangeLanguage(string languageCode)
        {
            var newLanguage = _availableLanguages.FirstOrDefault(x => x.LanguageCode!.Equals(languageCode, StringComparison.OrdinalIgnoreCase));
            if (newLanguage is not null && newLanguage != _currentLanguage)
            {
                _settings.Language = languageCode;
                _currentLanguage = newLanguage;
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(languageCode);
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(languageCode);
                OnLanguageChanged?.Invoke();
            }
        }
        #endregion Language


        #region Skin
        private AppSkinModel GetStartSkin()
        {
            return _availableSkins.FirstOrDefault(x =>
                            x.Name!.Equals((_utils.GetSystemTheme() ?? _settings.Skin), StringComparison.OrdinalIgnoreCase)
                        ) ?? _availableSkins.First();
        }

        public List<AppSkinModel> AvailableSkins()
        {
            return _availableSkins;
        }

        public void ChangeSkin(string skin)
        {
            var newSkin = _availableSkins.FirstOrDefault(x => x.Name!.Equals(skin, StringComparison.OrdinalIgnoreCase));
            if (newSkin is not null && newSkin != _currentSkin)
            {
                _settings.Skin = skin;
                _currentSkin = newSkin;
                OnSkinChanged?.Invoke();
            }
        }
        #endregion Skin
    }
}
