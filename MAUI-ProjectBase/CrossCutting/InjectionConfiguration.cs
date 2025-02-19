using AutoMapper;
using Data.ApiRepositories;
using Data.DatabaseRepositories;
using Domain;
using Domain.Interfaces.ApplicationConfigurationInterfaces;
using Domain.Models.ApplicationConfigurationModels;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.AuthenticationServices;

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
            serviceCollection.AddTransient<ISettingsServices, SettingsServices>();
        }

        public static void ConfigureDependenciesRepository(IServiceCollection serviceCollection)
        {
            #region DataBase
            serviceCollection.AddTransient<DefaultDatabaseAccess>();
            #endregion

            #region API
            serviceCollection.AddTransient<DefaultApiAccess>();
            #endregion
        }

        public static void ConfigureDependenciesExtras(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<AppUtils>();
            #region Authentication
            serviceCollection.AddTransient<IAccountServices, AccountServices>();
            serviceCollection.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            serviceCollection.AddAuthorizationCore();
            #endregion
        }
    }
}
