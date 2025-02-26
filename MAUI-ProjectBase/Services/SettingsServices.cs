using Data.DatabaseRepositories.EntityFrameworkContexts;
using Domain;
using Domain.Entities;
using Domain.Interfaces.ApplicationConfigurationInterfaces;
using Domain.Models.ApplicationConfigurationModels;
using Microsoft.EntityFrameworkCore;
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

        private readonly IDbContextFactory<AppDbContext> _dbFactory;

        public SettingsServices(AppUtils utils,
                                List<AppLanguageModel> availableLanguages,
                                List<AppSkinModel> availableSkins,
                                IDbContextFactory<AppDbContext> dbFactory,
                                AppSettingsModel settings)
        {
            _settings = settings;
            _utils = utils;
            _availableLanguages = availableLanguages;
            _availableSkins = availableSkins;
            _dbFactory = dbFactory;
            _currentLanguage = GetStartLanguage();
            _currentSkin = GetStartSkin();
        }

        public void AddLog(string log)
        {
            using var context = _dbFactory.CreateDbContext();
            context.Logs.Add(new LogEntity { Message = log });
            context.SaveChanges();
        }
        public List<LogEntity> logEntities()
        {
            using var context = _dbFactory.CreateDbContext();
            var logs = context.Logs.ToList();
            return logs;
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
