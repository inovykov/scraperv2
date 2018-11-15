using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scheduler.Configurations;
using Shared.Services;

namespace Scheduler.Services
{
    public class WorkloadService : IWorkloadService
    {
        private readonly IDateTimeService _dateTimeService;
        private readonly JobsConfig _jobsConfig;
        private readonly ILogger<WorkloadService> _logger;

        private readonly object _lockObj = new object();

        private DateTime _recentChangeDateTime;
        
        public TimeSpan IndividualSagaItemTaskExecutionDelay { get; private set; }

        public WorkloadService(IDateTimeService dateTimeService, 
            IOptions<JobsConfig> jobsConfig, 
            ILogger<WorkloadService> logger)
        {
            _dateTimeService = dateTimeService;
            _logger = logger;
            _jobsConfig = jobsConfig?.Value ?? throw new ArgumentNullException(nameof(jobsConfig));

            IndividualSagaItemTaskExecutionDelay = _jobsConfig.UpdateInfoAboutTvShowAsyncTimeSpan;

            _recentChangeDateTime = _dateTimeService.UtcNow;
        }

        public void IncreaseDelayTime()
        {
            lock (_lockObj)
            {
                var timePassedSinceRecentChange = _dateTimeService.UtcNow.Subtract(_recentChangeDateTime);

                if (timePassedSinceRecentChange < _jobsConfig.MinimalIntervalBetweenDelayIncreasing)
                {
                    return;
                }

                _recentChangeDateTime = _dateTimeService.UtcNow;

                IndividualSagaItemTaskExecutionDelay = IndividualSagaItemTaskExecutionDelay.Add(TimeSpan.FromMilliseconds(_jobsConfig.IncreaseDelayStepMilliseconds));

                _logger.LogInformation($"Request rate has changed to {IndividualSagaItemTaskExecutionDelay}. Change time: {_recentChangeDateTime}");
            }
        }
    }
}