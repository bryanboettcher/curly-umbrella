using HWI.Internal.Persistence;
using Newtonsoft.Json;

namespace HWI.Internal.Serialization.Newtonsoft
{
    public class JsonNetSerializer<T> : IObjectSerializer<T>
    {
        public T Deserialize(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public string Serialize(T data)
        {
            return JsonConvert.SerializeObject(data, Formatting.None);
        }
    }
}