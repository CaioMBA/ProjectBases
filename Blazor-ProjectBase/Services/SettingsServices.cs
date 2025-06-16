using Data.Database.EntityFrameworkContexts;
using Domain;
using Domain.Enums;
using Domain.Interfaces.ApplicationConfigurationInterfaces;
using Domain.Models.ApplicationConfigurationModels;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Services
{
    public class SettingsServices : ISettingsServices
    {
        private readonly List<AppLanguageModel> _availableLanguages;
        private readonly List<AppThemeModel> _availableThemes;

        public event Action? OnLanguageChanged;
        public event Action? OnThemeChanged;

        public AppLanguageModel currentLanguage { get; private set; }
        public AppThemeModel currentTheme { get; private set; }



        public SettingsServices(IDbContextFactory<AppDbContext> dbFactory)
        {
            _availableLanguages = AvailableLanguages();
            _availableThemes = AvailableThemes();
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
            return
            [
                new AppLanguageModel
                {
                    LanguageName = "English",
                    LanguageCode = "en-US",
                    LanguageFlag = "/assets/images/languages/en-us.svg",
                    loading_text = "Loading",
                    not_found_p = "Oops! The page you are looking for does not exist.",
                    not_found_a = "Press to go back [ Home ] - Automatic in",
                    forbidden_p = "Oops! You don't have permission for this page.",
                    forbidden_a = "Press to go back [ Home ] - Automatic in",
                    login_username_input_label = "Username",
                    login_password_input_label = "Password",
                    sidebar_logout_button = "Logout",
                    sidebar_home = "Home"
                },
                new AppLanguageModel
                {
                    LanguageName = "Português",
                    LanguageCode = "pt-BR",
                    LanguageFlag = "/assets/images/languages/pt-br.svg",
                    loading_text = "Carregando",
                    not_found_p = "Opa! A página que está procurando não existe!",
                    not_found_a = "Pressione para voltar [ Início ] - Automático em",
                    forbidden_p = "Opa! Você não tem permissão para acessar está página",
                    forbidden_a = "Pressione para voltar [ Início ] - Automático em",
                    login_username_input_label = "Usuário",
                    login_password_input_label = "Senha",
                    sidebar_logout_button = "Desconectar",
                    sidebar_home = "Início"
                }
            ];
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
            return _availableThemes.FirstOrDefault()!;
        }

        public List<AppThemeModel> AvailableThemes()
        {
            return
            [
                new AppThemeModel
                {
                    Name = "Dark",
                    Theme = AppTheme.Dark,
                    Path = "/css/Themes/dark.css",
                    Icon = "/assets/images/themes/dark.png"
                },
                new AppThemeModel
                {
                    Name = "Light",
                    Theme = AppTheme.Light,
                    Path = "/css/Themes/light.css",
                    Icon = "/assets/images/themes/light.png"
                }
            ];
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
