namespace HWI.Internal.Persistence
{
    public interface ICreatable<T>
    {
        void Create(T existing);
    }
}