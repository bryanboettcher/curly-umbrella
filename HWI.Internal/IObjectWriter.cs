namespace HWI.Internal
{
    public interface IObjectWriter<TLocation, TData>
    {
        void Write(TLocation location, TData data);
    }
}