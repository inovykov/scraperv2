using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shared.Interfaces;
using Shared.Models.Integration;
using Shared.Services;

namespace IntegrationBl.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly IIntegrationDal _integrationDal;
        private readonly IDateTimeService _dateTimeService;
        private readonly IIntegrationSagaFactory _integrationSagaFactory;
        private readonly ITvShowUpdateService _tvShowUpdateService;

        public UpdateService(IIntegrationDal integrationDal, 
            IDateTimeService dateTimeService, 
            IIntegrationSagaFactory integrationSagaFactory,
            ITvShowUpdateService tvShowUpdateService)
        {
            _integrationDal = integrationDal;
            _dateTimeService = dateTimeService;
            _integrationSagaFactory = integrationSagaFactory;
            _tvShowUpdateService = tvShowUpdateService;
        }

        public async Task<bool> StartUpdateProcessAsync(CancellationToken cancellationToken)
        {
            var uncompletedSagas =  await _integrationDal.GetSagaInProgressAsync(cancellationToken);

            if (uncompletedSagas != null)
            {
                return false;
            }

            var tvShowsToUpdateIds = await _tvShowUpdateService.GetOutdatedTvShowInfosIdsAsync(cancellationToken);

            if (tvShowsToUpdateIds == null || !tvShowsToUpdateIds.Any())
            {
                return false;
            }

            var newIntegrationSaga = _integrationSagaFactory.CreateIntegrationSaga(tvShowsToUpdateIds, _dateTimeService.UtcNow);

            await _integrationDal.SaveSagaAsync(newIntegrationSaga, cancellationToken);

            return true;
        }

        public async Task UpdateInfoAboutTvShowAsync(CancellationToken cancellationToken)
        {
            var saga = await _integrationDal.GetSagaInProgressAsync(cancellationToken);

            if (saga == null)
            {
                return;
            }

            var sagaItem = await _integrationDal.GetRandomSagaItemAsync(saga.Id, cancellationToken) ?? await _integrationDal.GetSingleSagaItemBySagaIdAsync(saga.Id, cancellationToken);

            if (sagaItem == null) // all items processed
            {
                await _integrationDal.SetSagaStateAsync(saga.Id, SagaStates.Completed, cancellationToken);

                return;
            }

            await _tvShowUpdateService.CreateOrUpdateTvShowAsync(sagaItem, cancellationToken);

            await _integrationDal.DeleteSagaItemByIdAsync(sagaItem.Id, cancellationToken);
        }
    }
}
