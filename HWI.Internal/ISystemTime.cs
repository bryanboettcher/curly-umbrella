using System;

namespace HWI.Internal
{
    public interface ISystemTime
    {
        DateTime Now();
        DateTime UtcNow();
    }
}