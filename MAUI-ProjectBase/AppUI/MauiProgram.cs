using Domain.Extensions;
using Domain.Models.ApplicationConfigurationModels;

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

            #region IF-DEBUG
#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            #endregion

            return builder.Build();
        }

        private static async Task<AppSettingsModel> LoadConfigurations()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("AboutAssets.txt");
            using var reader = new StreamReader(stream);

            string content = reader.ReadToEnd();

            return content.ToObject<AppSettingsModel>();
        }
    }
}
