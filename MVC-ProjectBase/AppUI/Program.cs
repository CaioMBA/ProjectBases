using CrossCutting;
using Data.Database.Contexts;
using Domain.Models.ApplicationModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

AppSettingsModel appSettings = new AppSettingsModel();

new ConfigureFromConfigurationOptions<AppSettingsModel>(
              builder.Configuration.GetSection("Settings"))
                  .Configure(appSettings);

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => { })
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
    options.SlidingExpiration = true;
    options.LoginPath = "/Login/Index";
    options.LogoutPath = "/Login/Index";
    options.AccessDeniedPath = "/AccessDenied/Index";
    options.ReturnUrlParameter = "/Home/Index";
});

AllConfigurations.ConfigureDependencies(builder.Services, appSettings);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
