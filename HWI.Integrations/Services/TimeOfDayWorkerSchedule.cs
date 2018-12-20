using System;

namespace HWI.Integrations.Services
{
    public abstract class TimeOfDayWorkerSchedule
    {
        private readonly TimeSpan _schedule;

        protected TimeOfDayWorkerSchedule(DateTime schedule)
        {
            _schedule = schedule.TimeOfDay;
        }

        protected TimeOfDayWorkerSchedule(TimeSpan schedule)
        {
            _schedule = schedule;
        }

        public bool ShouldRun(DateTime now, DateTime last)
        {
            return (now.Date > last.Date)
                   && now.TimeOfDay >= _schedule;
        }
    }
}