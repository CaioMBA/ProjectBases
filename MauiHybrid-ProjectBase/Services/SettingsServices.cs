using Data.DatabaseRepositories.EntityFrameworkContexts;
using Domain;
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
        private readonly List<AppThemeModel> _availableThemes;

        public event Action? OnLanguageChanged;
        public event Action? OnThemeChanged;

        public AppLanguageModel currentLanguage { get; private set; }
        public AppThemeModel currentTheme { get; private set; }


        public SettingsServices(AppUtils utils,
                                List<AppLanguageModel> availableLanguages,
                                List<AppThemeModel> availableThemes,
                                IDbContextFactory<AppDbContext> dbFactory,
                                AppSettingsModel settings)
        {
            _utils = utils;
            _settings = settings;
            _availableLanguages = availableLanguages;
            _availableThemes = availableThemes;
            currentLanguage = GetStartLanguage();
            currentTheme = GetStartTheme();
        }

        #region Language
        private AppLanguageModel GetStartLanguage()
        {
            return _availableLanguages.FirstOrDefault(x =>
                    x.LanguageCode!.Equals(CultureInfo.CurrentCulture.Name, StringComparison.OrdinalIgnoreCase)
                ) ?? _availableLanguages.FirstOrDefault()!;
        }

        public List<AppLanguageModel> AvailableLanguages()
        {
            return _availableLanguages;
        }

        public void ChangeLanguage(string languageCode)
        {
            var newLanguage = _availableLanguages.FirstOrDefault(x => x.LanguageCode!.Equals(languageCode, StringComparison.OrdinalIgnoreCase));
            if (newLanguage is not null && newLanguage != currentLanguage)
            {
                currentLanguage = newLanguage;
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(languageCode);
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(languageCode);
                OnLanguageChanged?.Invoke();
            }
        }
        #endregion Language


        #region Theme
        private AppThemeModel GetStartTheme()
        {
            return _availableThemes.FirstOrDefault(x =>
                x.Theme == _utils.GetSystemTheme()
                        ) ?? _availableThemes.FirstOrDefault()!;
        }

        public List<AppThemeModel> AvailableThemes()
        {
            return _availableThemes;
        }

        public void ChangeTheme(string theme)
        {
            var newTheme = _availableThemes.FirstOrDefault(x => x.Name!.Equals(theme, StringComparison.OrdinalIgnoreCase));
            if (newTheme is not null && newTheme != currentTheme)
            {
                currentTheme = newTheme;
                OnThemeChanged?.Invoke();
            }
        }
        #endregion Skin
    }
}
