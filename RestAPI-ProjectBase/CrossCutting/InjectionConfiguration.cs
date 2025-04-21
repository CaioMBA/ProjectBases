using Data.Api;
using Data.Database;
using Data.Database.EntityFrameworkContexts;
using Domain;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Mappings;
using Domain.Models.ApplicationConfigurationModels;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Services;


namespace CrossCutting
{
    public static class InjectionConfiguration
    {
        public static void ConfigureDependencies(IServiceCollection serviceCollection,
                                                 AppSettingsModel appSettings)
        {
            serviceCollection.AddSingleton<AppSettingsModel>(appSettings);
            ConfigureDependenciesExtras(serviceCollection);
            ConfigureDependenciesHealth(serviceCollection, appSettings);
            ConfigureAutoMapper(serviceCollection);
            ConfigureDependenciesService(serviceCollection);
            ConfigureDependenciesRepository(serviceCollection);
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
            serviceCollection.AddTransient<IExampleService, ExampleService>();
        }

        public static void ConfigureDependenciesRepository(IServiceCollection serviceCollection)
        {
            #region DataBase
            #region Entity Framework
            serviceCollection.AddDbContextFactory<AppDbContext>();
            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
                    using var dbContext = dbFactory.CreateDbContext();

                    dbContext.Database.Migrate();
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
            serviceCollection.AddMemoryCache();
            serviceCollection.AddSingleton<Utils>();
        }

        public static void ConfigureDependenciesStartup(IServiceCollection serviceCollection)
        {

        }
        public static void ConfigureDependenciesHealth(IServiceCollection serviceCollection,
                                                       AppSettingsModel settings)
        {
            var healthChecksBuilder = serviceCollection.AddHealthChecks();

            foreach (var db in settings.DataBaseConnectionModels ?? new())
            {
                var name = $"DB-{db.DataBaseID}";

                switch (db.Type)
                {
                    case DataBaseType.SQLSERVER:
                        healthChecksBuilder.AddSqlServer(
                            db.ConnectionString,
                            name: name,
                            tags: new[] { "db", "sqlserver", "critical" });
                        break;
                    case DataBaseType.POSTGRESQL:
                        healthChecksBuilder.AddNpgSql(
                            db.ConnectionString,
                            name: name,
                            tags: new[] { "db", "postgresql", "critical" });
                        break;
                    case DataBaseType.MYSQL:
                    case DataBaseType.MARIADB:
                        healthChecksBuilder.AddMySql(
                            db.ConnectionString,
                            name: name,
                            tags: new[] { "db", "mysql", "critical" });
                        break;
                    case DataBaseType.ORACLE:
                        healthChecksBuilder.AddOracle(
                            db.ConnectionString,
                            name: name,
                            tags: new[] { "db", "oracle", "critical" });
                        break;
                    default:
                        break;
                }
            }

            foreach (var api in settings.ApiConnections ?? new())
            {
                var baseUrl = new Uri(api.Url);
                var healthUrl = new Uri(baseUrl, "health");
                var name = $"API-{api.ApiID}";

                healthChecksBuilder.AddUrlGroup(
                    uri: healthUrl,
                    name: name,
                    tags: new[] { "api", "external", "critical" },
                    httpMethod: HttpMethod.Get);
            }

            healthChecksBuilder.AddCheck("SELF", () => HealthCheckResult.Healthy(), tags: new[] { "api", "critical" });
            healthChecksBuilder.AddDbContextCheck<AppDbContext>("AppDbContext", tags: new[] { "db", "critical", "efcore" });

            serviceCollection.AddHealthChecksUI(options =>
            {
                options.SetEvaluationTimeInSeconds(10);
                options.MaximumHistoryEntriesPerEndpoint(60);
                options.AddHealthCheckEndpoint("Default Health Check", "/health");
                options.AddHealthCheckEndpoint("Live Health Check", "/live");
                options.AddHealthCheckEndpoint("Ready Health Check", "/ready");
            }).AddInMemoryStorage();
        }
    }
}
