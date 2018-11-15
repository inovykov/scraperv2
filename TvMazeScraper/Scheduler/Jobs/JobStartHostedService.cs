using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scheduler.Clients;
using Scheduler.Configurations;

namespace Scheduler.Jobs
{
    public class JobStartHostedService : IHostedService
    {
        private readonly ILogger<JobStartHostedService> _logger; 
        private readonly IIntegrationClient _integrationClient;
        private readonly JobsConfig _integrationClientConfig;
        
        public JobStartHostedService(IIntegrationClient integrationClient, 
            IOptions<JobsConfig> integrationClientConfig, 
            ILogger<JobStartHostedService> logger)
        {
            _logger = logger;
            
            _integrationClient = integrationClient;
            _integrationClientConfig = integrationClientConfig?.Value ?? throw new ArgumentNullException(nameof(integrationClientConfig));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("Job started");

            await StartUpdateProcessAsync(cancellationToken); // TODO[EN] create recurring job starting immediately

            RecurringJob.AddOrUpdate(() => StartUpdateProcessAsync(cancellationToken), Cron.MinuteInterval(_integrationClientConfig.StartUpdateProcessTaskMinutesInterval));
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
