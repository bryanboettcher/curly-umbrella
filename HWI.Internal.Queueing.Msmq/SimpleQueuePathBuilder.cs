using System;

namespace HWI.Internal.Queueing.Msmq
{
    public class SimpleQueuePathBuilder : IQueuePathBuilder
    {
        public string BuildMsmqPath(Type targetObject)
        {
            var location = ".\\private$";
            var assemblyName = targetObject.Assembly.GetName().Name.ToLowerInvariant();
            var typeName = targetObject.Name.ToLowerInvariant();

            return $@"{location}\{assemblyName}.{typeName}";
        }
    }
}