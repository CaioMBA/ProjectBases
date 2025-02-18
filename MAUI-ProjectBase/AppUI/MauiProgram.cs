using Domain.Extensions;
using Domain.Models.ApplicationConfigurationModels;
using CrossCutting;
using Microsoft.Extensions.Logging;
using Domain.Interfaces.ApplicationConfiguration;

namespace AppUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            ConfigureMauiPlatformDependencies(builder.Services);

            AppSettingsModel? appSettings = LoadConfigurations().Result;
            if (appSettings == null)
            {
                throw new Exception("AppSettingsModel not found");
            }
            List<AppLanguageModel> availableLanguages = LoadAvailableLanguages(builder.Services).Result;

            InjectionConfiguration.ConfigureDependencies(builder.Services, appSettings, availableLanguages);

            #region IF-DEBUG
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif
            #endregion

            return builder.Build();
        }

        private static void ConfigureMauiPlatformDependencies(IServiceCollection serviceCollection)
        {
#if ANDROID
            serviceCollection.AddSingleton<IAssetService, AppUI.Platforms.Android.AssetService>();
#elif IOS
            serviceCollection.AddSingleton<IAssetService, AppUI.Platforms.iOS.AssetService>();
#elif WINDOWS
            serviceCollection.AddSingleton<IAssetService, AppUI.Platforms.Windows.AssetService>();
#endif
        }

        private static async Task<AppSettingsModel?> LoadConfigurations()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("appsettings.json");
            using var reader = new StreamReader(stream);

            string content = reader.ReadToEnd();

            return content.ToObject<AppSettingsModel>();
        }

        private static async Task<List<AppLanguageModel>> LoadAvailableLanguages(IServiceCollection serviceCollection)
        {
            var assetService = serviceCollection.BuildServiceProvider().GetRequiredService<IAssetService>();

            IEnumerable<string> assets = await assetService.ListAssetsAsync();

            List<AppLanguageModel> languages = new();
            foreach (string file in assets)
            {
                if (file.ToLower().Trim().StartsWith("language") && file.ToLower().Trim().EndsWith(".json"))
                {
                    using var stream = await FileSystem.OpenAppPackageFileAsync(file);
                    using var reader = new StreamReader(stream);
                    string content = reader.ReadToEnd();
                    var languageModel = content.ToObject<AppLanguageModel>();
                    if (languageModel != null)
                    {
                        languages.Add(languageModel);
                    }
                }
            }

            return languages;
        }
    }
}
