using System;
using System.Diagnostics;
using log4net;

namespace HWI.Internal.Logging.log4net
{
    public class Log4NetLogger : ILogger
    {
        private readonly ILog _logger;

        public Log4NetLogger(ILog logger)
        {
            _logger = logger;
        }

        [DebuggerStepThrough]
        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        [DebuggerStepThrough]
        public void Info(string message)
        {
            _logger.Info(message);
        }

        [DebuggerStepThrough]
        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        [DebuggerStepThrough]
        public void Error(string message)
        {
            _logger.Error(message);
        }

        [DebuggerStepThrough]
        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        [DebuggerStepThrough]
        public void Warn(string message, Exception e)
        {
            _logger.Warn(message, e);
        }

        [DebuggerStepThrough]
        public void Error(string message, Exception e)
        {
            _logger.Error(message, e);
        }

        [DebuggerStepThrough]
        public void Fatal(string message, Exception e)
        {
            _logger.Fatal(message, e);
        }
    }
}