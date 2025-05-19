using Data.DatabaseRepositories.EntityFrameworkContexts;
using Domain.Models.ApplicationConfigurationModels;
using Microsoft.EntityFrameworkCore;

namespace AppUI
{
    public partial class App : Application
    {
        private readonly AppSettingsModel _appSettings;
        public App(AppSettingsModel appSettings)
        {
            _appSettings = appSettings;
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = _appSettings.AppName };
        }
    }
}
