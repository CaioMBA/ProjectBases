using CrossCutting;
using Domain.Models.ApplicationConfigurationModels;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

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
    string swaggerJsonUrl = $"/swagger/v{appSettings.AppVersion}/swagger.json";

    app.MapOpenApi(swaggerJsonUrl);
    app.UseSwagger();
    app.UseSwaggerUI(opt => opt.SwaggerEndpoint(swaggerJsonUrl, appSettings.AppName));
}

app.UseCors(opt => opt.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

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
