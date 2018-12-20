namespace HWI.Internal.Persistence
{
    public interface IDeletable<T>
    {
        bool Delete(T item);
    }
}