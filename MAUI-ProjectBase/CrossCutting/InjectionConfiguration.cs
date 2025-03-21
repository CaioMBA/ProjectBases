﻿using AutoMapper;
using Data.ApiRepositories;
using Data.DatabaseRepositories.EntityFrameworkContexts;
using Domain;
using Domain.Interfaces.ApplicationConfigurationInterfaces;
using Domain.Models.ApplicationConfigurationModels;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.AuthenticationServices;

namespace CrossCutting
{
    public static class InjectionConfiguration
    {
        public static void ConfigureDependencies(IServiceCollection serviceCollection,
                                                 AppSettingsModel appSettings,
                                                 List<AppLanguageModel> availableLanguages,
                                                 List<AppSkinModel> availableSkins)
        {
            serviceCollection.AddSingleton(appSettings);
            serviceCollection.AddSingleton(availableLanguages);
            serviceCollection.AddSingleton(availableSkins);
            ConfigureDependenciesExtras(serviceCollection);
            ConfigureAutoMapper(serviceCollection);
            ConfigureDependenciesService(serviceCollection);
            ConfigureDependenciesRepository(serviceCollection);
            ConfigureDependenciesStartup(serviceCollection);
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
            serviceCollection.AddSingleton<ISettingsServices, SettingsServices>();
        }

        public static void ConfigureDependenciesRepository(IServiceCollection serviceCollection)
        {
            #region DataBase
            serviceCollection.AddDbContextFactory<AppDbContext>();
            #endregion

            #region API
            serviceCollection.AddTransient<DefaultApiAccess>();
            #endregion
        }

        public static void ConfigureDependenciesExtras(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<AppUtils>();

        }

        public static void ConfigureDependenciesStartup(IServiceCollection serviceCollection)
        {
            #region Authentication
            serviceCollection.AddScoped<IAccountServices, AccountServices>();
            serviceCollection.AddSingleton<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            serviceCollection.AddAuthorizationCore();
            #endregion
        }
    }
}
