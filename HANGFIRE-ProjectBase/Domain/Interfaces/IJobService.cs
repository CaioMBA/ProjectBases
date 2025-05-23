using Domain.Models.HangFireModels;
using Hangfire;
using Hangfire.Server;
using System.ComponentModel;

namespace Domain.Interfaces
{
    public interface IJobService
    {
        private const int _maxAttempts = 5;
        private const int _timeoutMinutes = 3;

        List<HangfireJobModel> GetProcessingJobs();
        HangfireStatisticsModel GetJobStatistics();

        [DisplayName("RunFireAndForgetJob")]//use brackets and their positions to show the parameters, example: {0} would be the parameter on this function [ context ] 
        [DisableConcurrentExecution(timeoutInSeconds: 60 * _timeoutMinutes)]
        [AutomaticRetry(Attempts = _maxAttempts, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        void RunFireAndForgetJob(PerformContext? context);

        [DisplayName("RunDelayedJob")]//use brackets and their positions to show the parameters, example: {0} would be the parameter on this function [ context ] 
        [DisableConcurrentExecution(timeoutInSeconds: 60 * _timeoutMinutes)]
        [AutomaticRetry(Attempts = _maxAttempts, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        void RunDelayedJob(PerformContext? context);

        [DisplayName("RunRecurringJob")]//use brackets and their positions to show the parameters, example: {0} would be the parameter on this function [ context ] 
        [DisableConcurrentExecution(timeoutInSeconds: 60 * _timeoutMinutes)]
        [AutomaticRetry(Attempts = _maxAttempts, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        void RunRecurringJob(PerformContext? context);
    }
}
