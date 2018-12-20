using System;

namespace HWI.Internal
{
    public class SimpleSystemTime : ISystemTime
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }

        public DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}