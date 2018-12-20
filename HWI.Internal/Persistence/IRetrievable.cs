using System.Collections.Generic;

namespace HWI.Internal.Persistence
{
    public interface IRetrievable<TKey, TOut>
    {
        TOut Retrieve(TKey key);
    }

    public interface IRetrievable<TKey1, TKey2, TOut>
    {
        TOut Retrieve(TKey1 key1, TKey2 key2);
    }

    public interface IRetrievable<TKey1, TKey2, TKey3, TOut>
    {
        TOut Retrieve(TKey1 key1, TKey2 key2, TKey3 key3);
    }
}