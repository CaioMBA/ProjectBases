using Data.DatabaseRepositories.EntityFrameworkContexts;
using Domain.Models.ApplicationConfigurationModels;
using Microsoft.EntityFrameworkCore;

namespace AppUI
{
    public partial class App : Application
    {
        private readonly AppSettingsModel _appSettings;
        private readonly AppDbContext _dbContext;
        public App(AppSettingsModel appSettings,
                   AppDbContext dbContext)
        {
            _appSettings = appSettings;
            _dbContext = dbContext;
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = _appSettings.AppName };
        }
        protected override void OnStart()
        {
            _dbContext.Database.EnsureCreated();
            #region IF-DEBUG
#if DEBUG
            _dbContext.Database.EnsureDeleted();
#endif
            #endregion
            _dbContext.Database.Migrate();
        }
    }
}
