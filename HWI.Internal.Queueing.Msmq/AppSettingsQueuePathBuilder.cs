using System;

namespace HWI.Internal.Queueing.Msmq
{
    public class AppSettingsQueuePathBuilder : IQueuePathBuilder
    {
        private readonly IPersistenceSettings _appSettings;

        public AppSettingsQueuePathBuilder(IPersistenceSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public string BuildMsmqPath(Type targetObject)
        {
            var typeName = targetObject.Name.ToLowerInvariant();
            var queueName = string.Concat(_appSettings.QueueLocation, _appSettings.QueueNamePrefix, ".", typeName);
            return queueName.Replace("..", ".");
        }
    }
}