using CrossCutting;
using Domain.Models.ApplicationConfigurationModels;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.AllowTrailingCommas = true;
    options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

#region Injection Configuration
var appSettings = new AppSettingsModel();
new ConfigureFromConfigurationOptions<AppSettingsModel>(
              builder.Configuration.GetSection("Settings"))
                  .Configure(appSettings);

InjectionConfiguration.ConfigureDependencies(builder.Services, appSettings);
#endregion

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc($"v{appSettings.AppVersion}", new OpenApiInfo { Title = appSettings.AppName, Version = appSettings.AppVersion.ToString() });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi($"/swagger/v{appSettings.AppVersion}/swagger.json");
    app.UseSwagger();
    app.UseSwaggerUI(opt => opt.SwaggerEndpoint($"/swagger/v{appSettings.AppVersion}/swagger.json", appSettings.AppName));
}

app.UseCors(opt => opt.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = $"{appSettings.AppName} v{appSettings.AppVersion}",
    StatsPollingInterval = 1000,
    Authorization = new[]
    {
        new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
        {
            SslRedirect = false,
            RequireSsl = false,
            LoginCaseSensitive = true,
            Users = new[]
            {
                new BasicAuthAuthorizationUser
                {
                    Login = appSettings.Hangfire?.AuthorizationCredential?.User ?? "admin",
                    PasswordClear = appSettings.Hangfire?.AuthorizationCredential?.Password ?? "password"
                }
            }
        })
    }
});

app.UseHttpsRedirection();

#region Health Checks
app.MapHealthChecks(Environment.GetEnvironmentVariable("HEALTHCHECK_PATH") ?? "/health", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
});

app.MapHealthChecks("/live", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("api"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/ready", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("db"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
    options.ResourcesPath = "/health-ui-resources";
    options.WebhookPath = "/health-api";
});
#endregion

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
