namespace HWI.Integrations.Services
{
    public interface IMessageProcessor<in TMessage> : IMessageProcessor where TMessage : BaseQueueableMessage, new()
    {
        void ProcessMessage(TMessage message);
    }

    public interface IMessageProcessor
    {
        int Parallelism { get; }
        string Description { get; }
    }
}
