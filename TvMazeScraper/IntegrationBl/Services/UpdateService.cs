using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationBl.Factories;
using Microsoft.Extensions.Logging;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Models.Integration;
using Shared.Services;

namespace IntegrationBl.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly IIntegrationDal _integrationDal;
        private readonly IDateTimeService _dateTimeService;
        private readonly IIntegrationTaskFactory _integrationTaskFactory;
        private readonly ITvShowUpdateService _tvShowUpdateService;
        private readonly IWorkloadService _workloadHandler;
        private readonly IPolicyFactory _policyFactory;
        private readonly ILogger<UpdateService> _logger;

        public UpdateService(IIntegrationDal integrationDal, 
            IDateTimeService dateTimeService, 
            IIntegrationTaskFactory integrationTaskFactory,
            ITvShowUpdateService tvShowUpdateService,
            IWorkloadService workloadHandler, 
            IPolicyFactory policyFactory, 
            ILogger<UpdateService> logger)
        {
            _integrationDal = integrationDal;
            _dateTimeService = dateTimeService;
            _integrationTaskFactory = integrationTaskFactory;
            _tvShowUpdateService = tvShowUpdateService;
            _workloadHandler = workloadHandler;
            _policyFactory = policyFactory;
            _logger = logger;
        }

        public async Task StartUpdateProcessAsync(CancellationToken cancellationToken)
        {
            var uncompletedTask =  await _integrationDal.GetTaskInProgressAsync(cancellationToken);

            if (uncompletedTask != null)
            {
                return;
            }

            var tvShowsToUpdateIds = await _tvShowUpdateService.GetOutdatedTvShowInfosIdsAsync(cancellationToken);

            if (tvShowsToUpdateIds.IsNullOrEmpty())
            {
                return;
            }

            var integrationTask = _integrationTaskFactory.CreateIntegrationTask(tvShowsToUpdateIds, _dateTimeService.UtcNow);

            await _integrationDal.SaveIntegrationTaskAsync(integrationTask, cancellationToken);
            
            var policies = _policyFactory.CreateUpdateTaskPolicies(() => _workloadHandler.IncreaseDelayTime());

            try
            {

                foreach (var tvShowToUpdateId in tvShowsToUpdateIds)
                {
                    await Task.Delay(_workloadHandler.UpdateTvShowInfoTaskExecutionDelay, cancellationToken);

                    await policies.ExecuteAsync(async () =>
                        await _tvShowUpdateService.CreateOrUpdateTvShowAsync(tvShowToUpdateId, cancellationToken));
                }
            }

            catch (Exception ex)
            {
                _logger.LogError("Update task failed", ex);

                await _integrationDal.SetIntegrationTaskStateAsync(integrationTask.Id, IntegrationTaskStates.Failed, cancellationToken);
            }

            await _integrationDal.SetIntegrationTaskStateAsync(integrationTask.Id, IntegrationTaskStates.Completed, cancellationToken);
        }
    }
}
