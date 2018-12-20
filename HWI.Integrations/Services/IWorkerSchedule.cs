using System;
using System.Threading;
using System.Threading.Tasks;

namespace HWI.Integrations.Services
{
    public interface IWorkerSchedule
    {
        bool ShouldRun(DateTime now, DateTime last);
        Task Run(CancellationToken token);
        bool RunImmediately { get; }
    }
}