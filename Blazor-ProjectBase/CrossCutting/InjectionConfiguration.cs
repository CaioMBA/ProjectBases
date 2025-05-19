using Dapper;
using Data.Api;
using Data.Database;
using Data.Database.DapperHandlers;
using Data.Database.EntityFrameworkContexts;
using Domain;
using Domain.Mappings;
using Domain.Models.ApplicationConfigurationModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting
{
    public static class InjectionConfiguration
    {
        public static void ConfigureDependencies(IServiceCollection serviceCollection,
                                                 AppSettingsModel appSettings)
        {
            serviceCollection.AddSingleton<AppSettingsModel>(appSettings);
            ConfigureDependenciesExtras(serviceCollection);
            ConfigureAutoMapper(serviceCollection);
            ConfigureDependenciesService(serviceCollection);
            ConfigureDependenciesRepository(serviceCollection).Wait();
        }

        public static void ConfigureAutoMapper(IServiceCollection serviceCollection)
        {
            serviceCollection.AddAutoMapper(
                typeof(EntityToModelMapping).Assembly,
                typeof(ModelToDtoMapping).Assembly
                );
        }

        public static void ConfigureDependenciesService(IServiceCollection serviceCollection)
        {
        }

        public static async Task ConfigureDependenciesRepository(IServiceCollection serviceCollection)
        {
            #region DataBase
            #region Dapper Type Handlers
            SqlMapper.AddTypeHandler(new JsonArrayTypeHandler());
            SqlMapper.AddTypeHandler(new JsonObjectTypeHandler());
            SqlMapper.AddTypeHandler(new JsonNodeTypeHandler());
            #endregion
            #region Entity Framework
            serviceCollection.AddDbContextFactory<AppDbContext>();
            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();

                    using (var dbContext = await dbFactory.CreateDbContextAsync())
                    {
                        await dbContext.Database.MigrateAsync();
                    }
                }
            }
            #endregion
            serviceCollection.AddTransient<DefaultDatabaseAccess>();
            #endregion

            #region API
            serviceCollection.AddTransient<DefaultApiAccess>();
            #endregion
        }

        public static void ConfigureDependenciesExtras(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            serviceCollection.AddMemoryCache();
            serviceCollection.AddSingleton<Utils>();
        }

        public static void ConfigureDependenciesStartup(IServiceCollection serviceCollection)
        {

        }
    }
}
