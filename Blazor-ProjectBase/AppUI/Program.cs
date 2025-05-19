using AppUI.Components;
using CrossCutting;
using Domain.Models.ApplicationConfigurationModels;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

#region Injection Configuration
var appSettings = new AppSettingsModel();
new ConfigureFromConfigurationOptions<AppSettingsModel>(
              builder.Configuration.GetSection("Settings"))
                  .Configure(appSettings);

InjectionConfiguration.ConfigureDependencies(builder.Services, appSettings);
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseCors(opt => opt.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
