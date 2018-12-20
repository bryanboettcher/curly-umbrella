using System;

namespace HWI.Internal.Queueing
{
    public class DeadLetter
    {
        /// <summary>
        /// This will never be set by application code, only by the storage engine.
        /// Use it if it makes your dead-letter handling easier
        /// </summary>
        public string Id { get; set; }
        public string Payload { get; set; }
        public Exception Exception { get; set; }
        public string OriginalQueue { get; set; }
        public DateTime CaughtTime { get; set; }
        public string ComputerName { get; set; }
    }
}