using System;

namespace HWI.Internal.Logging
{
    public interface ILogger
    {
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Fatal(string message);

        void Warn(string message, Exception e);
        void Error(string message, Exception e);
        void Fatal(string message, Exception e);
    }
}