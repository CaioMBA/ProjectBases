using Domain.Extensions;
using Domain.Models.ApplicationConfigurationModels;
using CrossCutting;
using Microsoft.Extensions.Logging;
using Domain.Interfaces.ApplicationConfigurationInterfaces;
using System.Reflection.PortableExecutable;

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
                    fonts.AddFont("Kadwa-Regular.ttf", "Kadwa");
                    fonts.AddFont("Kadwa-Bold.ttf", "KadwaBold");
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
            serviceCollection.AddSingleton<IAssetServices, AppUI.Platforms.Android.AssetService>();
#elif IOS
            serviceCollection.AddSingleton<IAssetServices, AppUI.Platforms.iOS.AssetService>();
#elif WINDOWS
            serviceCollection.AddSingleton<IAssetServices, AppUI.Platforms.Windows.AssetService>();
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
            var assetService = serviceCollection.BuildServiceProvider().GetRequiredService<IAssetServices>();

            IEnumerable<string> assets = await assetService.ListAssetsAsync();
            IEnumerable<string> languageAssets = assets.Where(x => x.ToLower().Trim().StartsWith("language") && x.ToLower().Trim().EndsWith(".json"));

            List<AppLanguageModel> languages = new();
            foreach (string file in languageAssets)
            {
                string content = assetService.ReadAssetContent(file);
                var languageModel = content.ToObject<AppLanguageModel>();
                if (languageModel != null)
                {
                    string fileName = file.Split(Path.DirectorySeparatorChar).Last()
                                          .Split(Path.AltDirectorySeparatorChar).Last()
                                          .ToLower().Replace(".json", "");
                    languageModel.LanguageCode = fileName;
                    languageModel.LanguageFlag = $"flag_{fileName}.png";

                    languages.Add(languageModel);
                }
            }

            return languages;
        }
    }
}
