using System;

namespace HWI.Integrations
{
    public class MessageProcessorInstrumentation
    {
        public string MessageProcessor { get; set; }
        public long ExecutionTime { get; set; }
        public long SessionMessages { get; set; }
        public long SessionExceptions { get; set; }
        public DateTime InstrumentationTime { get; set; }

        public override string ToString()
        {
            return $@"{{
    ""MessageProcessor"":""{MessageProcessor}"",
    ""ExecutionTime"":""{ExecutionTime}"",
    ""SessionMessages"":""{SessionMessages}"",
    ""SessionExceptions"":""{SessionExceptions}"",
""InstrumentationTime"":""{InstrumentationTime}""
}}";
        }
    }
}