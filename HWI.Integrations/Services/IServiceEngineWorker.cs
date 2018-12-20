using System.Threading;
using System.Threading.Tasks;

namespace HWI.Integrations.Services
{
    public interface IServiceEngineWorker
    {
        Task Start(CancellationToken token, int instance);
        int Parallelism { get; }
    }
}