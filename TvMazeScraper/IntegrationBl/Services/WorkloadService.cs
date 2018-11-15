using System;
using IntegrationBl.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Services;

namespace IntegrationBl.Services
{
    public class WorkloadService : IWorkloadService
    {
        private readonly IDateTimeService _dateTimeService;
        private readonly UpdateTasksConfig _updateTasksConfig;
        private readonly ILogger<WorkloadService> _logger;

        private readonly object _lockObj = new object();

        private DateTime _recentChangeDateTime;
        
        public TimeSpan UpdateTvShowInfoTaskExecutionDelay { get; private set; }

        public WorkloadService(IDateTimeService dateTimeService, 
            IOptions<UpdateTasksConfig> jobsConfig, 
            ILogger<WorkloadService> logger)
        {
            _dateTimeService = dateTimeService;
            _logger = logger;
            _updateTasksConfig = jobsConfig?.Value ?? throw new ArgumentNullException(nameof(jobsConfig));

            UpdateTvShowInfoTaskExecutionDelay = _updateTasksConfig.UpdateInfoAboutTvShowAsyncTimeSpan;

            _recentChangeDateTime = _dateTimeService.UtcNow;
        }

        public void IncreaseDelayTime()
        {
            lock (_lockObj)
            {
                var timePassedSinceRecentChange = _dateTimeService.UtcNow.Subtract(_recentChangeDateTime);

                if (timePassedSinceRecentChange < _updateTasksConfig.MinimalIntervalBetweenDelayIncreasing)
                {
                    return;
                }

                _recentChangeDateTime = _dateTimeService.UtcNow;

                UpdateTvShowInfoTaskExecutionDelay = UpdateTvShowInfoTaskExecutionDelay.Add(TimeSpan.FromMilliseconds(_updateTasksConfig.IncreaseDelayStepMilliseconds));

                _logger.LogInformation($"Request rate has changed to {UpdateTvShowInfoTaskExecutionDelay}. Change time: {_recentChangeDateTime}");
            }
        }
    }
}