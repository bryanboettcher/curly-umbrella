using System;
using System.Collections.Generic;
using System.Text;
using HWI.Internal.Queueing;

namespace HWI.Internal.Extensions
{
    public static class MessageExtensions
    {
        public static IEnumerable<T> ReadAll<T>(this IMessageReader<T> messageReader) where T : class 
        {
            while (true)
            {
                var item = messageReader.Read();
                if (item == default(T))
                    yield break;

                yield return item;
            }
        }
    }
}
