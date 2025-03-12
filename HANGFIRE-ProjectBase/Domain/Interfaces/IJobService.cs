using Domain.Models.HangFireModels;
using Hangfire.Server;

namespace Domain.Interfaces
{
    public interface IJobService
    {
        List<HangfireJobModel> GetProcessingJobs();
        HangfireStatisticsModel GetJobStatistics();
        void RunFireAndForgetJob(PerformContext? context);
        void RunDelayedJob(PerformContext? context);
        void RunRecurringJob(PerformContext? context);
    }
}
