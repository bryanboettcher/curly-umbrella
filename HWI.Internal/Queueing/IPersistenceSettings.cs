namespace HWI.Internal.Queueing
{
    public interface IPersistenceSettings
    {
        string QueueNamePrefix { get; }
        string QueueLocation { get; }
    }
}