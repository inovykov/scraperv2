using System.Threading;
using System.Threading.Tasks;

namespace IntegrationBl.Services
{
    public interface IUpdateService
    {
        Task<bool> StartUpdateProcessAsync(CancellationToken cancellationToken);

        Task UpdateInfoAboutTvShowAsync(CancellationToken cancellationToken);
    }
}