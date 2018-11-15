using System;
using System.Threading;
using System.Threading.Tasks;
using Shared.Models.Integration;

namespace Shared.Interfaces
{
    public interface IIntegrationDal
    {
        Task<IntegrationTask> GetTaskInProgressAsync(CancellationToken cancellationToken);

        Task<IntegrationTask> GetMostRecentCompletedTaskAsync(CancellationToken cancellationToken);

        Task SaveIntegrationTaskAsync(IntegrationTask integrationTask, CancellationToken cancellationToken);

        Task SetIntegrationTaskStateAsync(Guid id, IntegrationTaskStates state, CancellationToken cancellationToken);
    }
}