using System;

namespace HWI.Internal.Queueing.Msmq
{
    public interface IQueuePathBuilder
    {
        string BuildMsmqPath(Type targetObject);
    }
}