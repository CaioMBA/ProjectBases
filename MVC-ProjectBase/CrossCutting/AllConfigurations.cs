using AutoMapper;
using Data.Database.Contexts;
using Domain;
using Domain.Models.ApplicationModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting
{
    public class AllConfigurations
    {
        public static void ConfigureDependencies(IServiceCollection serviceCollection, AppSettingsModel appConfig)
        {
            serviceCollection.AddSingleton(appConfig);
            ConfigureAutoMapper(serviceCollection);
            ConfigureDependenciesService(serviceCollection);
            ConfigureDependenciesRepository(serviceCollection);
            ConfigureDependenciesExtras(serviceCollection);
            ConfigureDependenciesEntityFramework(serviceCollection);
            ConfigureDependencyIdentity(serviceCollection);

            ConfigureStartup(serviceCollection);
        }

        public static void ConfigureAutoMapper(IServiceCollection serviceCollection)
        {
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
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
        }

        public static void ConfigureDependenciesExtras(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ApplicationUtils>();
        }

        public static void ConfigureDependenciesEntityFramework(IServiceCollection serviceCollection)
        {
            ApplicationUtils? utils = serviceCollection.BuildServiceProvider().GetService<ApplicationUtils>();

            DataBaseConnectionModel? AuthDb = utils!.GetDataBase("Default");

            serviceCollection.AddDbContextFactory<ApplicationDbContext>(
                options => options.UseSqlServer(AuthDb.ConnectionString,
                                            b => b.MigrationsAssembly("Data"))
                );

            serviceCollection.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(AuthDb.ConnectionString,
                                            b => b.MigrationsAssembly("Data"))
                );
        }

        public static void ConfigureDependencyIdentity(IServiceCollection serviceCollection)
        {
            serviceCollection.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
            });
        }
        public static void ConfigureStartup(IServiceCollection serviceCollection)
        {
        }
    }
}
