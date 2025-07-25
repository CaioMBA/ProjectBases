using Domain.Interfaces.StateInterfaces;
using Domain.Models.ApplicationConfigurationModels;

namespace AppUI
{
    public partial class App : Application
    {
        private readonly AppSettingsModel _appSettings;
        private readonly IRefreshViewState _refreshViewState;
        public App(AppSettingsModel appSettings, IRefreshViewState refreshViewState)
        {
            _appSettings = appSettings;
            _refreshViewState = refreshViewState;
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage(_refreshViewState)) { Title = _appSettings.AppName };
        }
    }
}
