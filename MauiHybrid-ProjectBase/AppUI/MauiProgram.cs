using CommunityToolkit.Maui;
using CrossCutting;
using Domain;
using Domain.Extensions;
using Domain.Interfaces.ApplicationConfigurationInterfaces;
using Domain.Models.ApplicationConfigurationModels;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;

namespace AppUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseLocalNotification()
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

            List<AppThemeModel> availableSkins = LoadAvailableSkins(builder.Services).Result;

            InjectionConfiguration.ConfigureDependencies(builder.Services, appSettings, availableLanguages, availableSkins);

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
            serviceCollection.AddSingleton<IPlatformSpecificServices, AppUI.Platforms.Android.PlatformSpecificServices>();
#elif IOS
            serviceCollection.AddSingleton<IPlatformSpecificServices, AppUI.Platforms.iOS.PlatformSpecificServices>();
#elif WINDOWS
            serviceCollection.AddSingleton<IPlatformSpecificServices, AppUI.Platforms.Windows.PlatformSpecificServices>();
#endif
        }

        private static async Task<AppSettingsModel?> LoadConfigurations()
        {
            using (var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").Result)
            {
                using (var reader = new StreamReader(stream))
                {
                    string content = await reader.ReadToEndAsync();

                    return content.ToObject<AppSettingsModel>();
                }
            }
        }

        private static async Task<List<AppLanguageModel>> LoadAvailableLanguages(IServiceCollection serviceCollection)
        {
            var assetService = serviceCollection.BuildServiceProvider().GetRequiredService<IPlatformSpecificServices>();

            IEnumerable<string> assets = await assetService.ListAssetsAsync();

            List<AppLanguageModel?> languages = assets.Where(x => x.Trim().StartsWith("language", StringComparison.OrdinalIgnoreCase)
                                                                        && x.Trim().EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                                                            .Select(file =>
                                                            {
                                                                string content = assetService.ReadAssetContent(file);
                                                                var languageModel = content.ToObject<AppLanguageModel>();
                                                                if (languageModel != null)
                                                                {
                                                                    string fileName = Path.GetFileNameWithoutExtension(file).ToLower();
                                                                    languageModel.LanguageCode = fileName;
                                                                    languageModel.LanguageFlag = $"{fileName}.svg";
                                                                    return languageModel;
                                                                }
                                                                return null;
                                                            }).ToList();

            return languages.Where(x => x is not null).Distinct().ToList()!;
        }

        private static async Task<List<AppThemeModel>> LoadAvailableSkins(IServiceCollection serviceCollection)
        {
            var assetService = serviceCollection.BuildServiceProvider().GetRequiredService<IPlatformSpecificServices>();

            IEnumerable<string> assets = await assetService.ListAssetsAsync();
            List<AppThemeModel> skins = assets.Where(x =>
                                                (x.Trim().StartsWith(@"wwwroot/css/skins", StringComparison.OrdinalIgnoreCase)
                                                 ||
                                                 x.Trim().StartsWith(@"wwwroot\css\skins", StringComparison.OrdinalIgnoreCase))
                                                && x.Trim().EndsWith(@".css", StringComparison.OrdinalIgnoreCase))
                                        .Select(x =>
                                        {
                                            var name = x
                                                      .Replace(@"wwwroot/css/skins/", "", StringComparison.OrdinalIgnoreCase)
                                                      .Replace(@"wwwroot\css\skins\", "", StringComparison.OrdinalIgnoreCase)
                                                      .Replace(@".css", "", StringComparison.OrdinalIgnoreCase);

                                            return new AppThemeModel()
                                            {
                                                Name = name,
                                                Path = x.ToLower(),
                                                Theme = name.ToAppTheme(),
                                                Icon = $"{name}_skin.png"
                                            };
                                        })
                                        .Distinct()
                                        .ToList();
            return skins.Where(x => x is not null).Distinct().ToList()!;
        }
    }
}
