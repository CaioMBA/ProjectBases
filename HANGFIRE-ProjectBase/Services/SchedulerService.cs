using AutoMapper;
using Domain;
using Domain.Interfaces;
using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class SchedulerService : CustomService<SchedulerService>, ISchedulerService, IHostedService
    {
        public SchedulerService(IJobService jobService,
                                ILogger<SchedulerService> logger,
                                IMapper mapper,
                                Utils utils)
                         : base(logger, mapper, utils)
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            BackgroundJob.Enqueue<IJobService>(x => x.RunFireAndForgetJob(null));

            BackgroundJob.Schedule<IJobService>(x => x.RunDelayedJob(null), TimeSpan.FromMinutes(10));

            RecurringJob.AddOrUpdate<IJobService>("recurring-job", x => x.RunRecurringJob(null), Cron.Minutely);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
