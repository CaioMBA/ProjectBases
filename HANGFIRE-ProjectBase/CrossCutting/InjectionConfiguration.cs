using AutoMapper;
using Data.Api;
using Data.Database;
using Domain;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models.ApplicationConfigurationModels;
using Hangfire;
using Hangfire.Console;
using Hangfire.MemoryStorage;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Hangfire.MySql;
using Hangfire.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Services;
using System;

namespace CrossCutting
{
    public static class InjectionConfiguration
    {
        public static void ConfigureDependencies(IServiceCollection serviceCollection,
                                                 AppSettingsModel appSettings)
        {
            serviceCollection.AddSingleton(appSettings);
            ConfigureDependenciesExtras(serviceCollection);
            ConfigureAutoMapper(serviceCollection);
            ConfigureDependenciesService(serviceCollection);
            ConfigureDependenciesRepository(serviceCollection);
            ConfigureDependenciesHealth(serviceCollection, appSettings);
            ConfigureDependenciesStartup(serviceCollection, appSettings);
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
            serviceCollection.AddTransient<IJobService, JobService>();
            serviceCollection.AddTransient<ISchedulerService, SchedulerService>();
        }

        public static void ConfigureDependenciesRepository(IServiceCollection serviceCollection)
        {
            #region DataBase
            serviceCollection.AddTransient<DefaultDatabaseAccess>();
            #endregion

            #region API
            serviceCollection.AddHttpClient();
            serviceCollection.AddTransient<DefaultApiAccess>();
            #endregion
        }

        public static void ConfigureDependenciesExtras(IServiceCollection serviceCollection)
        {
            serviceCollection.AddMemoryCache();
            serviceCollection.AddSingleton<Utils>();
        }

        public static void ConfigureDependenciesStartup(IServiceCollection serviceCollection,
                                                        AppSettingsModel appSettings)
        {
            var utils = serviceCollection.BuildServiceProvider().GetRequiredService<Utils>();
            var hangfireDatabase = utils.GetDataBase("HangFire");
            serviceCollection.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                          .UseSimpleAssemblyNameTypeSerializer()
                          .UseRecommendedSerializerSettings()
                          .UseConsole();

                switch (hangfireDatabase.Type)
                {
                    case DataBaseType.SQLSERVER:
                        config.UseSqlServerStorage(hangfireDatabase.ConnectionString, new SqlServerStorageOptions());
                        break;
                    case DataBaseType.MARIADB:
                    case DataBaseType.MYSQL:
                        config.UseStorage(new MySqlStorage(hangfireDatabase.ConnectionString, new MySqlStorageOptions
                        {
                            TablesPrefix = $"{appSettings.AppName?.Trim().ToLower().Replace(" ", string.Empty)}_",
                            QueuePollInterval = TimeSpan.FromMilliseconds(1500)
                        }));
                        break;

                    case DataBaseType.MONGODB:
                        config.UseMongoStorage(hangfireDatabase.ConnectionString, new MongoStorageOptions
                        {
                            MigrationOptions = new MongoMigrationOptions
                            {
                                MigrationStrategy = new MigrateMongoMigrationStrategy(),
                                BackupStrategy = new CollectionMongoBackupStrategy()
                            },
                            Prefix = $"{appSettings.AppName?.Trim().ToLower().Replace(" ", string.Empty)}_",
                            CheckConnection = true,
                            QueuePollInterval = TimeSpan.FromMilliseconds(1500)
                        });
                        break;
                    default:
                        config.UseMemoryStorage();
                        break;
                }
            });


            serviceCollection.AddHangfireServer(options =>
            {
                options.ServerName = $"{utils.GetMachineName()} - {appSettings.Hangfire?.Identifier ?? utils.GetMachineHostName()}";
                options.WorkerCount = appSettings.Hangfire?.WorkerCount ?? 1;
            });

            serviceCollection.AddHostedService<SchedulerService>();
        }

        public static void ConfigureDependenciesHealth(IServiceCollection serviceCollection,
                                                       AppSettingsModel settings)
        {
            var healthChecksBuilder = serviceCollection.AddHealthChecks();

            foreach (var db in settings.DataBaseConnections ?? new())
            {
                var name = $"DB-{db.DataBaseID}";
                if (String.IsNullOrEmpty(db.ConnectionString))
                {
                    continue;
                }

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
                    case DataBaseType.MARIADB:
                    case DataBaseType.MYSQL:
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
                    case DataBaseType.MONGODB:
                        var mongoUrl = new MongoUrl(db.ConnectionString);
                        var mongoClient = new MongoClient(mongoUrl);

                        healthChecksBuilder.AddMongoDb(
                            clientFactory: _ => mongoClient,
                            databaseNameFactory: _ => mongoUrl.DatabaseName ?? "admin",
                            name: name,
                            tags: new[] { "db", "mongodb", "critical" });
                        break;
                    default:
                        break;
                }
            }

            foreach (var api in settings.ApiConnections ?? new())
            {
                var apiUrl = new Uri(api.Url);
                var healthUrl = new Uri(apiUrl, "health");
                var name = $"API-{api.ApiID}";

                healthChecksBuilder.AddUrlGroup(
                    uri: healthUrl,
                    name: name,
                    tags: new[] { "api", "external", "critical" },
                    httpMethod: HttpMethod.Get);
            }

            healthChecksBuilder.AddCheck(name: "SELF", () => HealthCheckResult.Healthy(), tags: new[] { "api", "critical" });

            bool inContainer = string.Equals(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), "true", StringComparison.OrdinalIgnoreCase);

            var baseUrl = inContainer ? Environment.GetEnvironmentVariable("HEALTHCHECK_BASEURL") ?? "http://localhost:8080" : string.Empty;

            serviceCollection.AddHealthChecksUI(options =>
            {
                options.SetEvaluationTimeInSeconds(10);
                options.MaximumHistoryEntriesPerEndpoint(60);
                options.AddHealthCheckEndpoint("Default Health Check", $"{baseUrl}{Environment.GetEnvironmentVariable("HEALTHCHECK_PATH") ?? "/health"}");
                options.AddHealthCheckEndpoint("Live Health Check", $"{baseUrl}/live");
                options.AddHealthCheckEndpoint("Ready Health Check", $"{baseUrl}/ready");
            }).AddInMemoryStorage();
        }
    }
}
