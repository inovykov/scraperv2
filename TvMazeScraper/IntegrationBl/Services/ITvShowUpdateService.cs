using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shared.Models.Integration;

namespace IntegrationBl.Services
{
    public interface ITvShowUpdateService
    {
        Task CreateOrUpdateTvShowAsync(IntegrationItem sagaItem, CancellationToken cancellationToken);

        Task<IList<int>> GetOutdatedTvShowInfosIdsAsync(CancellationToken cancellationToken);
    }
}