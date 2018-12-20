using System;
using Experimental.System.Messaging;

namespace HWI.Internal.Queueing.Msmq
{
    public abstract class MsmqDeadlLetterHandler
    {
        public virtual string Name => this.GetType().Name;

        protected void ReplayMessageWithMsmq(DeadLetter message)
        {
            var originalMessage = message.Payload;

            var originalQueue = message.OriginalQueue;
            if (message.ComputerName != Environment.MachineName)
            {
                if (originalQueue.StartsWith("."))
                    originalQueue = "FormatName:Direct=OS:" + message.ComputerName + originalQueue.Substring(1);
            }

            var queue = new MessageQueue(originalQueue);
            queue.Formatter = new BinaryMessageFormatter();
            queue.Send(originalMessage);
            queue.Close();
        }
    }
}