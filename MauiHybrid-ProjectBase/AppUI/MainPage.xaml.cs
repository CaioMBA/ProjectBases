using Domain.Interfaces.StateInterfaces;

namespace AppUI
{
    public partial class MainPage : ContentPage
    {
        private readonly IRefreshViewState _refreshState;

        public MainPage(IRefreshViewState refreshState)
        {
            InitializeComponent();
            _refreshState = refreshState;
            DataTemplate selectedTemplate = GetTemplateForPlatform();
            View content = (View)selectedTemplate.CreateContent();
            content.BindingContext = _refreshState;
            RootContentView.Content = content;
        }

        private DataTemplate GetTemplateForPlatform()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android
                || DeviceInfo.Platform == DevicePlatform.iOS)
            {
                return (DataTemplate)Resources["WithRefreshTemplate"];
            }
            return (DataTemplate)Resources["WithoutRefreshTemplate"];
        }

        private void OnRefresh(object? sender, EventArgs? e)
        {
            _refreshState.Refresh(sender, e);
        }
    }
}
