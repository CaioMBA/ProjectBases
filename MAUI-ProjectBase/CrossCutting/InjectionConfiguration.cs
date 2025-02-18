using AutoMapper;
using Domain;
using Domain.Models.ApplicationConfigurationModels;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting
{
    public class InjectionConfiguration
    {
        public static void ConfigureDependencies(IServiceCollection serviceCollection,
                                                 AppSettingsModel appSettings,
                                                 List<AppLanguageModel> availableLanguages)
        {
            serviceCollection.AddSingleton(appSettings);
            serviceCollection.AddSingleton(availableLanguages);
            ConfigureAutoMapper(serviceCollection);
            ConfigureDependenciesService(serviceCollection);
            ConfigureDependenciesRepository(serviceCollection);
            ConfigureDependenciesExtras(serviceCollection);
        }

        public static void ConfigureAutoMapper(IServiceCollection serviceCollection)
        {
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                //cfg.AddProfile(new ModelToDto());
                //cfg.AddProfile(new DtoToModel());
            });
            IMapper mapper = config.CreateMapper();
            serviceCollection.AddSingleton(mapper);
        }

        public static void ConfigureDependenciesService(IServiceCollection serviceCollection)
        {
        }

        public static void ConfigureDependenciesRepository(IServiceCollection serviceCollection)
        {
            #region DataBase
            #endregion

            #region API
            #endregion
        }

        public static void ConfigureDependenciesExtras(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<AppUtils>();
        }
    }
}
