namespace HWI.Internal.Queueing
{
    public interface IMessageReader<TMessage>
    {
        TMessage Read();
    }
}