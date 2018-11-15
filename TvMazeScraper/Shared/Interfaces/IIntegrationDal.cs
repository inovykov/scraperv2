using System;
using System.Threading;
using System.Threading.Tasks;
using Shared.Models.Integration;

namespace Shared.Interfaces
{
    public interface IIntegrationDal
    {
        Task<IntegrationSaga> GetSagaInProgressAsync(CancellationToken cancellationToken);

        Task<IntegrationSaga> GetMostRecentCompletedSagaAsync(CancellationToken cancellationToken);

        Task SaveSagaAsync(IntegrationSagaExtended integrationSaga, CancellationToken cancellationToken);

        Task SetSagaStateAsync(Guid id, SagaStates state, CancellationToken cancellationToken);

        Task<IntegrationItem> GetSingleSagaItemBySagaIdAsync(Guid sagaId, CancellationToken cancellationToken);

        Task<IntegrationItem> GetRandomSagaItemAsync(Guid sagaId, CancellationToken cancellationToken);
        
        Task DeleteSagaItemByIdAsync(int id, CancellationToken cancellationToken);
    }
}