using AppUI.Components;
using AppUI.UiServices;
using CrossCutting;
using Domain.Interfaces.BlazorUiInterfaces;
using Domain.Models.ApplicationConfigurationModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

#region Injection Configuration
builder.Services.Configure<AppSettingsModel>(
    builder.Configuration.GetSection("Settings")
);
builder.Services.AddScoped<IBlazorStorageService, BlazorSessionStorageService>();
await InjectionConfiguration.ConfigureDependencies(builder.Services);
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
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

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
