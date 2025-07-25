using Domain.Interfaces.StateInterfaces;

namespace AppUI
{
    public partial class MainPage : ContentPage
    {
        private readonly IRefreshViewState _refreshState;
        public MainPage(IRefreshViewState refreshState)
        {
            _refreshState = refreshState;
            InitializeComponent();
            BindingContext = _refreshState;
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            _refreshState.Refresh(sender, e);
        }
    }
}
