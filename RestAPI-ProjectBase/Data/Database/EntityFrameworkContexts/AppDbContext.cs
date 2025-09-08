using Data.Database.EntityFrameworkContexts.Converters;
using Data.Database.EntityFrameworkContexts.ValueGenerators;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NCrontab;
using System.Text.Json.Nodes;

namespace Data.Database.EntityFrameworkContexts
{
    public class AppDbContext() : DbContext()
    {
        private readonly DbContextOptions<AppDbContext>? _options = new();
        private readonly string _dbPath = Path.Combine(Directory.GetCurrentDirectory(), "default_app.db");

        public DbSet<LogEntity> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Filename={_dbPath}", b => b.MigrationsAssembly("Data"));
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties().Where(p => p.IsKey() && p.ClrType == typeof(Guid)))
                {
                    property.SetValueGeneratorFactory((prop, type) => new GuidV7ValueGenerator());
                }
            }
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            string provider = Database.ProviderName ?? _options?.ContextType.Name ?? string.Empty;
            string jsonColumnType = provider.Trim().ToUpperInvariant() switch
            {
                var p when p.Contains("NPGSQL") => "jsonb",
                var p when p.Contains("MYSQL") => "json",
                var p when p.Contains("MARIA") => "json",
                var p when p.Contains("SQLITE") => "TEXT",
                _ => "varchar(max)"
            };
            string guidColumnType = provider.Trim().ToUpperInvariant() switch
            {
                var p when p.Contains("NPGSQL") => "uuid",
                var p when p.Contains("SQLSERVER") => "uniqueidentifier",
                var p when p.Contains("MYSQL") => "char(36)",
                var p when p.Contains("MARIA") => "char(36)",
                var p when p.Contains("SQLITE") => "TEXT",
                _ => "char(36)"
            };

            // CronExpression
            configurationBuilder.Properties<CrontabSchedule>().HaveConversion<CronExpressionConverter>().HaveColumnType("varchar(100)");
            configurationBuilder.Properties<CrontabSchedule?>().HaveConversion<NullableCronExpressionConverter>().HaveColumnType("varchar(100)");

            // DateOnly
            configurationBuilder.Properties<DateOnly>().HaveConversion<DateOnlyConverter>().HaveColumnType("date");
            configurationBuilder.Properties<DateOnly?>().HaveConversion<NullableDateOnlyConverter>().HaveColumnType("date");

            // Json
            configurationBuilder.Properties<JsonArray>().HaveConversion<JsonArrayConverter>().HaveColumnType(jsonColumnType);
            configurationBuilder.Properties<JsonArray?>().HaveConversion<NullableJsonArrayConverter>().HaveColumnType(jsonColumnType);
            configurationBuilder.Properties<JsonNode>().HaveConversion<JsonNodeConverter>().HaveColumnType(jsonColumnType);
            configurationBuilder.Properties<JsonNode?>().HaveConversion<NullableJsonNodeConverter>().HaveColumnType(jsonColumnType);
            configurationBuilder.Properties<JsonObject>().HaveConversion<JsonObjectConverter>().HaveColumnType(jsonColumnType);
            configurationBuilder.Properties<JsonObject?>().HaveConversion<NullableJsonObjectConverter>().HaveColumnType(jsonColumnType);

            // Guid
            configurationBuilder.Properties<Guid>().HaveConversion<GuidV7Converter>().HaveColumnType(guidColumnType);
            configurationBuilder.Properties<Guid?>().HaveConversion<NullableGuidV7Converter>().HaveColumnType(guidColumnType);

            // TimeOnly
            configurationBuilder.Properties<TimeOnly>().HaveConversion<TimeOnlyConverter>().HaveColumnType("time");
            configurationBuilder.Properties<TimeOnly?>().HaveConversion<NullableTimeOnlyConverter>().HaveColumnType("time");
        }
    }
}
