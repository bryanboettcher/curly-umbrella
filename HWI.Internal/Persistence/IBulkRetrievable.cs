using System.Collections.Generic;

namespace HWI.Internal.Persistence
{
    public interface IBulkRetrievable<T>
    {
        IEnumerable<T> RetrieveAll();
    }

    public interface IBulkRetrievable<TKey, TOut>
    {
        IEnumerable<TOut> RetrieveAll(TKey key);
    }

    public interface IBulkRetrievable<TKey1, TKey2, TOut>
    {
        IEnumerable<TOut> RetrieveAll(TKey1 key1, TKey2 key2);
    }

    public interface IBulkRetrievable<TKey1, TKey2, TKey3, TOut>
    {
        IEnumerable<TOut> RetrieveAll(TKey1 key1, TKey2 key2, TKey3 key3);
    }
}