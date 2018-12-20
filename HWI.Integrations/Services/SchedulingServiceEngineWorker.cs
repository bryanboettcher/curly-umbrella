using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HWI.Internal;
using HWI.Internal.Logging;

namespace HWI.Integrations.Services
{
    public class SchedulingServiceEngineWorker : IServiceEngineWorker
    {
        private readonly IEnumerable<IWorkerSchedule> _workerSchedules;
        private readonly ILogger _logger;
        private readonly ISystemTime _systemTime;

        private readonly ConcurrentDictionary<IWorkerSchedule, DateTime> _lastRunTimes;

        public SchedulingServiceEngineWorker(
            IEnumerable<IWorkerSchedule> workerSchedules,
            ILogger logger,
            ISystemTime systemTime)
        {
            _workerSchedules = workerSchedules;
            _logger = logger;
            _systemTime = systemTime;

            _lastRunTimes = new ConcurrentDictionary<IWorkerSchedule, DateTime>();
        }

        public Task Start(CancellationToken token, int instance)
        {
            _logger.Info($"Schedule worker starting up, instance {instance}");

            return Task.Factory.StartNew(() => HandleScheduleLoop(token));
        }

        private void HandleScheduleLoop(CancellationToken token)
        {
            foreach (var schedule in _workerSchedules)
                _lastRunTimes.TryAdd(schedule, schedule.RunImmediately ? DateTime.MinValue : DateTime.Today);

            if (_lastRunTimes.Count == 0)
            {
                _logger.Info("No scheduled workers found, nothing to do ...");
                return;
            }

            _logger.Debug(_lastRunTimes.Count + " schedules registered");

            while (!token.IsCancellationRequested)
            {
                var now = _systemTime.Now();

                Task.Delay(1000).Wait();

                if (token.IsCancellationRequested) return;

                var nextRun = _workerSchedules
                    .Where(schedule => schedule.ShouldRun(now, _lastRunTimes[schedule]))
                    .ToList();

                if (nextRun.Any())
                {
                    _logger.Debug($"Schedule worker has {nextRun.Count} for the next run");
                }
                else
                {
                    continue;
                }

                Parallel.ForEach(nextRun, schedule => _lastRunTimes.AddOrUpdate(schedule, DateTime.MinValue, (k, e) => now));

                try
                {
                    Task.WaitAll(
                        nextRun
                            .Select(r => r.Run(token))
                            .ToArray()
                        , token);
                }
                catch (AggregateException ae)
                {
                    foreach (var e in ae.Flatten().InnerExceptions)
                        _logger.Error("Error while running schedule workers", e);
                }
                catch (Exception e)
                {
                    _logger.Error("Error while running schedule workers", e);
                }
            }

            _logger.Info("Gracefully shutting down");
        }

        public int Parallelism => 1;
    }
}