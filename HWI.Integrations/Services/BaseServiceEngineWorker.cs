using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using HWI.Internal;
using HWI.Internal.Logging;
using HWI.Internal.Persistence;
using HWI.Internal.Queueing;

namespace HWI.Integrations.Services
{
    public class BaseServiceEngineWorker<TMessage> : IServiceEngineWorker where TMessage : BaseQueueableMessage, new()
    {
        private readonly IMessageReader<TMessage> _messageReader;
        private readonly IMessageWriter<DeadLetter> _deadLetterMessageWriter;
        private readonly IMessageWriter<MessageProcessorInstrumentation> _instrumentationMessageWriter;
        private readonly IObjectSerializer<TMessage> _serializer;
        private readonly IMessageProcessor<TMessage> _messageProcessor;
        private readonly ISystemTime _systemTime;
        private readonly ILogger _logger;

        public BaseServiceEngineWorker(
            IMessageReader<TMessage> messageReader,
            IMessageWriter<DeadLetter> deadLetterMessageWriter,
            IMessageWriter<MessageProcessorInstrumentation> instrumentationMessageWriter,
            IObjectSerializer<TMessage> serializer,
            IMessageProcessor<TMessage> messageProcessor,
            ISystemTime systemTime,
            ILogger logger
        )
        {
            _messageReader = messageReader;
            _deadLetterMessageWriter = deadLetterMessageWriter;
            _instrumentationMessageWriter = instrumentationMessageWriter;
            _serializer = serializer;
            _messageProcessor = messageProcessor;
            _systemTime = systemTime;
            _logger = logger;

            if (messageProcessor.Parallelism == 0)
                throw new InvalidOperationException("Parallelism cannot be zero for message processor");
        }

        public Task Start(CancellationToken token, int instance)
        {
            return Task.Factory.StartNew(() => ProcessMessageLoop(token, instance));
        }

        private void ProcessMessageLoop(CancellationToken token, int instance)
        {
            var message = default(TMessage);
            var messageProcessorName = _messageProcessor.GetType().Name;

            long sessionMessages = 0;
            long sessionExceptions = 0;

            _logger.Info($"Service worker starting up, instance {instance}");

            while (!token.IsCancellationRequested)
            {
                var sw = Stopwatch.StartNew();

                try
                {
                    do
                    {
                        message = _messageReader.Read();
                        if (token.IsCancellationRequested)
                            break;
                    } while (message == null);

                    if (token.IsCancellationRequested)
                        break;

                    sw.Restart();
                    _logger.Debug("Received message, forwarding to processor");
                    _messageProcessor.ProcessMessage(message);

                    ++sessionMessages;

                    SendInstrumentationMessage(
                        messageProcessorName,
                        sw.ElapsedMilliseconds,
                        sessionMessages,
                        sessionExceptions
                    );
                }
                catch (AggregateException ae)
                {
                    foreach (var e in ae.Flatten().InnerExceptions)
                    {
                        SendDeadletterMessage(message, e);
                        ++sessionExceptions;
                    }
                }
                catch (Exception e)
                {
                    SendDeadletterMessage(message, e);
                    ++sessionExceptions;
                }
            }

            _logger.Info("Gracefully shutting down");
        }

        private void SendInstrumentationMessage(
            string messageProcessorName,
            long executionTime,
            long sessionMessages,
            long sessionExceptions)
        {
            _logger.Debug("Emitting instrumentation data");

            _instrumentationMessageWriter.Write(new MessageProcessorInstrumentation
            {
                MessageProcessor = messageProcessorName,
                ExecutionTime = executionTime,
                SessionMessages = sessionMessages,
                SessionExceptions = sessionExceptions,
                InstrumentationTime = _systemTime.Now()
            });
        }

        private void SendDeadletterMessage(TMessage message, Exception e)
        {
            var deadLetterMessage = new DeadLetter
            {
                Payload = _serializer.Serialize(message),
                Exception = e,
                OriginalQueue = _messageReader.ToString(),
                CaughtTime = _systemTime.Now(),
                ComputerName = Environment.MachineName
            };

            _logger.Error($"Caught exception during processing {e.Message}, writing dead-letter object", e);
            
            _deadLetterMessageWriter.Write(deadLetterMessage);
        }

        public int Parallelism => _messageProcessor.Parallelism;
    }
}