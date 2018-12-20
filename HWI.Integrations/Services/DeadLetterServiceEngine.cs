using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HWI.Integrations.Services.Recovery;
using HWI.Internal.Logging;
using HWI.Internal.Persistence;
using HWI.Internal.Queueing;

namespace HWI.Integrations.Services
{
    public class DeadLetterServiceEngine : IServiceEngine
    {
        private readonly IEnumerable<IDeadLetterHandler> _deadLetterHandlers;

        private readonly IMessageReader<DeadLetter> _deadLetterMessageReader;
        private readonly IObjectSerializer<DeadLetter> _serializer;
        private readonly ILogger _logger;

        private Task _workerTask;

        private CancellationTokenSource _tokenSource;

        public DeadLetterServiceEngine(
            IEnumerable<IDeadLetterHandler> deadLetterHandlers,
            IMessageReader<DeadLetter> deadLetterMessageReader,
            IObjectSerializer<DeadLetter> serializer,
            ILogger logger)
        {
            _deadLetterHandlers = deadLetterHandlers.ToList();
            _deadLetterMessageReader = deadLetterMessageReader;
            _serializer = serializer;
            _logger = logger;
        }

        public void Start()
        {
            _logger.Debug("Starting dead-letter handling engine");

            if (!_deadLetterHandlers.Any())
            {
                _logger.Error("No dead letter handlers found!  Nothing to do ...");
                return;
            }

            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;

            _workerTask = Task.Factory.StartNew(() => HandleDeadLetters(token));
        }

        public void Stop()
        {
            _logger.Debug("Stopping dead-letter handling engine");

            _tokenSource?.Cancel();
            _workerTask?.Wait();
        }

        private void HandleDeadLetters(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var message = _deadLetterMessageReader.Read();
                if (message == null) continue;

                _logger.Info("Dead letter message received, searching for handler");

                var messageHandler = _deadLetterHandlers
                    .OrderBy(dlh => dlh.ExecutionOrder)
                    .FirstOrDefault(dlh => dlh.CanHandle(message));

                if (messageHandler == null)
                {
                    _logger.Error("Unable to find any handler for message");
                    HailMarySave(_serializer.Serialize(message));
                    continue;
                }

                try
                {
                    _logger.Debug($"{messageHandler.Name} has offered to handle the dead letter message, passing on");
                    messageHandler.HandleMessage(message);
                }
                catch (Exception e)
                {
                    _logger.Error($"Message handler {messageHandler.Name} threw an exception, attempting to salvage deadletter message", e);
                    HailMarySave(_serializer.Serialize(message));
                }
            }

            _logger.Info("Gracefully shutting down");
        }

        private void HailMarySave(string message)
        {
            var path = Path.Combine(Environment.CurrentDirectory, Guid.NewGuid().ToString());

            try
            {
                _logger.Error($"Saving to {path}");
                File.WriteAllText(path, message);
            }
            catch (Exception e)
            {
                _logger.Fatal($"Hail-mary saves are broken.  Lost message is after this log", e);
                _logger.Fatal(message);

                Environment.FailFast("Hail-mary saves are broken.  Do not restart application without fixing dead-letter handling, or data will be lost", e);
            }
        }
    }
}