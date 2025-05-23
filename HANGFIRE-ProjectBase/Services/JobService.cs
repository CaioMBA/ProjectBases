﻿using AutoMapper;
using Domain;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models.HangFireModels;
using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace Services
{
    public class JobService : CustomService<JobService>, IJobService
    {
        public JobService(ILogger<JobService> logger,
                          IMapper mapper,
                          Utils utils)
            : base(logger, mapper, utils)
        {
        }

        #region API
        public List<HangfireJobModel> GetProcessingJobs()
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                var monitoringApi = JobStorage.Current.GetMonitoringApi();
                var processingJobs = monitoringApi.ProcessingJobs(0, int.MaxValue);

                List<HangfireJobModel> jobList = new List<HangfireJobModel>();

                foreach (var job in processingJobs)
                {
                    string jobId = job.Key;
                    string jobName = job.Value.Job.ToString();
                    jobList.Add(new HangfireJobModel()
                    {
                        Id = jobId,
                        Name = jobName
                    });
                }

                return jobList;
            }
        }

        public HangfireStatisticsModel GetJobStatistics()
        {
            if (JobStorage.Current == null)
                throw new InvalidOperationException("Hangfire JobStorage is not initialized.");

            using (var connection = JobStorage.Current.GetConnection())
            {
                var monitoringApi = JobStorage.Current.GetMonitoringApi();

                return new HangfireStatisticsModel()
                {
                    Succeeded = monitoringApi.SucceededJobs(0, int.MaxValue).Count,
                    Failed = monitoringApi.FailedCount(),
                    Processing = monitoringApi.ProcessingCount(),
                    Scheduled = monitoringApi.ScheduledCount(),
                    Enqueued = monitoringApi.Queues().Sum(q => monitoringApi.EnqueuedCount(q.Name)),
                    Deleted = monitoringApi.DeletedJobs(0, int.MaxValue).Count,
                    Retries = connection.GetAllItemsFromSet("retries").Count,
                    Servers = monitoringApi.Servers().Count
                };
            }
        }
        #endregion


        public void RunFireAndForgetJob(PerformContext? context)
        {
            CS_Log($"Fire-and-Forget Job Executed at {DateTime.UtcNow}", LogType.Information, context);
        }


        public void RunDelayedJob(PerformContext? context)
        {
            CS_Log($"Delayed Job Executed at {DateTime.UtcNow}", LogType.Information, context);
        }


        public void RunRecurringJob(PerformContext? context)
        {
            CS_Log($"Recurring Job Executed at {DateTime.UtcNow}", LogType.Information, context);
        }
    }
}
