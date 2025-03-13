using AutoMapper;
using Data.Api;
using Data.Database;
using Domain;
using Domain.Interfaces;
using Domain.Models.ApplicationConfigurationModels;
using Hangfire;
using Hangfire.Console;
using Hangfire.MemoryStorage;
using Hangfire.MySql;
using Hangfire.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Services;

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

                switch (hangfireDatabase.Type.ToUpper())
                {
                    case "SQLSERVER":
                        config.UseSqlServerStorage(hangfireDatabase.ConnectionString, new SqlServerStorageOptions());
                        break;
                    case "MYSQL":
                        config.UseStorage(new MySqlStorage(hangfireDatabase.ConnectionString, new MySqlStorageOptions
                        {
                            TablesPrefix = $"{appSettings.AppName?.Trim().ToLower().Replace(" ", string.Empty)}_",
                            QueuePollInterval = TimeSpan.FromMilliseconds(1500)
                        }));
                        break;
                    case "MARIADB":
                        config.UseStorage(new MySqlStorage(hangfireDatabase.ConnectionString, new MySqlStorageOptions
                        {
                            TablesPrefix = $"{appSettings.AppName?.Trim().ToLower().Replace(" ", string.Empty)}_",
                            QueuePollInterval = TimeSpan.FromMilliseconds(1500)
                        }));
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
    }
}
