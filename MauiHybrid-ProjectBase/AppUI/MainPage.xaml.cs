using Domain.Interfaces.StateInterfaces;

namespace AppUI
{
    public partial class MainPage : ContentPage
    {
        private readonly IRefreshViewState _refreshViewState;
        public MainPage(IRefreshViewState refreshState)
        {
            _refreshViewState = refreshState;
            InitializeComponent();
            BindingContext = _refreshViewState;
        }

        private void Refreshing(object? sender, EventArgs e)
        {
            _refreshViewState.IsRefreshing = true;
        }
    }
}
