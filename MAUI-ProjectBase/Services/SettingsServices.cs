﻿using Domain.Models.ApplicationConfigurationModels;
using Domain;
using Domain.Interfaces.ApplicationConfigurationInterfaces;

namespace Services
{
    public class SettingsServices : ISettingsServices
    {
        private readonly AppUtils _utils;
        private readonly AppSettingsModel _settings;
        private readonly List<AppLanguageModel> _availableLanguages;

        public event Action? OnLanguageChanged;
        public event Action? OnSkinChanged;

        public AppLanguageModel _currentLanguage { get; private set; }

        public SettingsServices(AppUtils utils,
                               List<AppLanguageModel> availableLanguages,
                               AppSettingsModel settings)
        {
            _settings = settings;
            _utils = utils;
            _availableLanguages = availableLanguages;
            _currentLanguage = _availableLanguages.FirstOrDefault(x => x.LanguageCode == (_settings.Language ?? "en-us"))!;
        }

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

        public void ChangeSkin(string skin)
        {
            _settings.Skin = skin;
            OnSkinChanged?.Invoke();
        }
    }
}
