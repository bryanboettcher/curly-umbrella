namespace HWI.Internal.Persistence
{
    public interface IObjectSerializer<T>
    {
        T Deserialize(string data);
        string Serialize(T data);
    }
}