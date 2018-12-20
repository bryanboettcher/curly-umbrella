using HWI.Internal.Queueing;

namespace HWI.Integrations.Services.Recovery
{
    public interface IDeadLetterHandler
    {
        string Name { get; }
        int ExecutionOrder { get; }

        bool CanHandle(DeadLetter message);
        void HandleMessage(DeadLetter message);
    }
}