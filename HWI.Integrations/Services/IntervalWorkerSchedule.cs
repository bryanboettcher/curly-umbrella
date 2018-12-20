using System;

namespace HWI.Integrations.Services
{
    public abstract class IntervalWorkerSchedule
    {
        private readonly TimeSpan _interval;

        protected IntervalWorkerSchedule(TimeSpan interval)
        {
            _interval = interval;
        }

        public bool ShouldRun(DateTime now, DateTime last)
        {
            return (now - last) > _interval;
        }
    }
}