using System.Threading;
using System.Threading.Tasks;

namespace Scheduler.Clients
{
    public interface IIntegrationClient
    {
        Task StartUpdateProcessAsync(CancellationToken cancellationToken);
    }
}