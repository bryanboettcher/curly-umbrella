namespace HWI.Internal.Persistence
{
    public interface IUpdatable<T>
    {
        void Update(T item);
    }
}