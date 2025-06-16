using Dapper;
using Data.Api;
using Data.Database;
using Data.Database.DapperHandlers;
using Data.Database.EntityFrameworkContexts;
using Domain;
using Domain.Interfaces.ApplicationConfigurationInterfaces;
using Domain.Mappings;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.AuthenticationServices;

namespace CrossCutting
{
    public static class InjectionConfiguration
    {
        public static async Task ConfigureDependencies(IServiceCollection serviceCollection)
        {
            ConfigureDependenciesExtras(serviceCollection);
            ConfigureAutoMapper(serviceCollection);
            ConfigureDependenciesService(serviceCollection);
            await ConfigureDependenciesRepository(serviceCollection);
            ConfigureDependenciesStartup(serviceCollection);
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
            serviceCollection.AddSingleton<ISettingsServices, SettingsServices>();
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
                        try
                        {
#if DEBUG
                            await dbContext.Database.EnsureDeletedAsync();
#endif
                            await dbContext.Database.MigrateAsync();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Database migration failed. Exception: {ex.Message}");
                        }
                    }
                }
            }
            #endregion
            serviceCollection.AddTransient<DefaultDatabaseAccess>();
            #endregion

            #region API
            serviceCollection.AddHttpClient();
            serviceCollection.AddTransient<DefaultApiAccess>();
            #endregion
        }

        public static void ConfigureDependenciesExtras(IServiceCollection serviceCollection)
        {
            #region Memory
            serviceCollection.AddMemoryCache();
            serviceCollection.AddDistributedMemoryCache();
            #endregion

            serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            serviceCollection.AddScoped<Utils>();
        }

        public static void ConfigureDependenciesStartup(IServiceCollection serviceCollection)
        {
            #region Authentication
            serviceCollection.AddScoped<IAccountServices, AccountServices>();
            serviceCollection.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            serviceCollection.AddAuthorizationCore();
            #endregion
        }
    }
}
