using Domain.Models.HangFireModels;

namespace Domain.Interfaces
{
    public interface ISchedulerService
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
