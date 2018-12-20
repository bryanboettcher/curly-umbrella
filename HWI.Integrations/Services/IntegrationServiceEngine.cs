using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HWI.Internal.Logging;

namespace HWI.Integrations.Services
{
    public class IntegrationServiceEngine : IServiceEngine
    {
        private readonly IEnumerable<IServiceEngineWorker> _workers;
        private readonly ILogger _logger;

        private Task[] _workerTasks;

        private CancellationTokenSource _tokenSource;

        public IntegrationServiceEngine(IEnumerable<IServiceEngineWorker> workers, ILogger logger)
        {
            _workers = workers;
            _logger = logger;
        }

        public void Start()
        {
            _logger.Debug("Starting service engine");

            _tokenSource = new CancellationTokenSource();

            var token = _tokenSource.Token;

            _workerTasks = _workers.SelectMany(
                    worker => Enumerable
                        .Range(1, worker.Parallelism)
                        .Select(instance => worker.Start(token, instance)))
                .ToArray();
        }

        public void Stop()
        {
            _tokenSource.Cancel();

            Task.WaitAll(_workerTasks);
        }
    }
}