using System;
using System.Messaging;
using Experimental.System.Messaging;
using HWI.Internal.Logging;
using HWI.Internal.Persistence;

namespace HWI.Internal.Queueing.Msmq
{
    public class BaseMessageReaderWriter<T> : IMessageWriter<T>, IMessageReader<T>
    {
        private readonly object _locker = new object();

        private readonly IObjectSerializer<T> _serializer;
        private readonly ILogger _logger;
        private readonly Lazy<MessageQueue> _queue;
        private readonly string _path;

        public BaseMessageReaderWriter(IObjectSerializer<T> serializer, ILogger logger, IQueuePathBuilder queuePathBuilder)
        {
            _serializer = serializer;
            _logger = logger;

            _path = queuePathBuilder.BuildMsmqPath(typeof(T));

            _queue = new Lazy<MessageQueue>(() => !QueueExists(_path)
                ? CreateMessageQueue(_path)
                : OpenMessageQueue(_path));
        }

        private MessageQueue OpenMessageQueue(string path)
        {
            _logger.Info($"Opening connection to existing queue, at {path}");
            var queue = new MessageQueue(path);
            
            return PrepareQueue(queue);
        }

        private MessageQueue CreateMessageQueue(string path)
        {
            _logger.Warn($"Queue not found, creating {path}");
            var queue = MessageQueue.Create(path);

#if NETSTANDARD2_0
#else
            queue.SetPermissions("ANONYMOUS LOGON", MessageQueueAccessRights.GenericWrite, AccessControlEntryType.Allow);
            queue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow);
#endif

            return PrepareQueue(queue);
        }

        private static MessageQueue PrepareQueue(MessageQueue queue)
        {
            queue.Formatter = new BinaryMessageFormatter();
            queue.DefaultPropertiesToSend.Recoverable = true;
            return queue;
        }

        private bool QueueExists(string path)
        {
            _logger.Debug($"Checking for MSMQ path {path}");
            return path.StartsWith("FormatName") || MessageQueue.Exists(path);
        }
        
        public void Write(T message)
        {
            var data = _serializer.Serialize(message);

            lock (_locker)
            {
                _logger.Debug($"Writing {data.Length} characters to {_path}");
                _queue.Value.Send(data);
            }
        }

        public T Read()
        {
            string messageBody;

            lock (_locker)
            {
                Message message;
                try
                {
                    message = _queue.Value.Receive(TimeSpan.FromMilliseconds(1000));
                }
                catch (MessageQueueException e)
                {
                    if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout) return default(T);

                    throw;
                }

                messageBody = (string) message.Body;
            }

            _logger.Debug($"Received {messageBody} from {_path}");
            
            return _serializer.Deserialize(messageBody);
        }

        public override string ToString()
        {
            return _queue.Value.Path;
        }
    }
}