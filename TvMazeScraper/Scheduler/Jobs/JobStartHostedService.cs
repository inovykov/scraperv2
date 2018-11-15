using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scheduler.Clients;
using Scheduler.Configurations;
using Scheduler.Services;
using Shared.Exceptions;

namespace Scheduler.Jobs
{
    public class JobStartHostedService : IHostedService
    {
        private readonly ILogger<JobStartHostedService> _logger; 
        private readonly IIntegrationClient _integrationClient;
        private readonly JobsConfig _integrationClientConfig;
        private readonly IWorkloadService _workloadHandler;
        
        public JobStartHostedService(IIntegrationClient integrationClient, 
            IOptions<JobsConfig> integrationClientConfig, 
            ILogger<JobStartHostedService> logger, 
            IWorkloadService workloadHandler)
        {
            _logger = logger;
            _workloadHandler = workloadHandler;
            _integrationClient = integrationClient;
            _integrationClientConfig = integrationClientConfig?.Value ?? throw new ArgumentNullException(nameof(integrationClientConfig));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("Job started");

            await StartUpdateProcessAsync(cancellationToken);

            RecurringJob.AddOrUpdate(() => StartUpdateProcessAsync(cancellationToken), Cron.MinuteInterval(_integrationClientConfig.StartUpdateProcessTaskMinutesInterval));
            
            // Hangfire doesn't support intervals which are less then a minute
            while (true)
            {
                await Task.Delay(_workloadHandler.IndividualSagaItemTaskExecutionDelay, cancellationToken);
                
                // fire and forget intentionally
                Task.Run(async() => await StartUpdateIndividualSagaItemProcess(cancellationToken));
            }
        }

        public async Task StartUpdateIndividualSagaItemProcess(CancellationToken cancellationToken)
        {
            _logger.LogTrace("Starting 'Update Info Task'");

            try
            {
                await _integrationClient.UpdateInfoAboutTvShowAsync(cancellationToken);
            }
            catch (TooManyRequestsException)
            {
                _workloadHandler.IncreaseDelayTime();
            }
        }
        
        public async Task StartUpdateProcessAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("Update process started");

            try
            {
                 await _integrationClient.StartUpdateProcessAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during starting updating process {ex.Message}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
