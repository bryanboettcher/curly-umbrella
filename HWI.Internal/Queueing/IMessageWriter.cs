namespace HWI.Internal.Queueing
{
    public interface IMessageWriter<TMessage>
    {
        void Write(TMessage message);
    }
}