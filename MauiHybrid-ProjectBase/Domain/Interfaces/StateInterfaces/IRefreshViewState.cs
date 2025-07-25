using System.ComponentModel;

namespace Domain.Interfaces.StateInterfaces
{
    public interface IRefreshViewState
    {
        bool IsRefreshing { get; set; }
        bool IsEnabled { get; set; }

        event PropertyChangedEventHandler? PropertyChanged;

        void Refresh(object? sender, EventArgs e);
    }
}
