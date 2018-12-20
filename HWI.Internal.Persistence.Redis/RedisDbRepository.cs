using System;
using StackExchange.Redis;

namespace HWI.Internal.Persistence.Redis
{
    public abstract class RedisDbRepository<T>
    {
        protected readonly IConnectionMultiplexer ConnectionMultiplexer;
        private readonly IObjectSerializer<T> _serializer;

        protected RedisDbRepository(IConnectionMultiplexer connectionMultiplexer, IObjectSerializer<T> serializer)
        {
            ConnectionMultiplexer = connectionMultiplexer;
            _serializer = serializer;
        }

        public virtual void Create(T item)
        {
            ValidateIdField(item);

            dynamic clone = item;
            clone.Id = Guid.NewGuid().ToString("D");

            var db = ConnectionMultiplexer.GetDatabase();

            OnSaveData(db, clone.Id, item);
        }

        public virtual void Update(T item)
        {
            ValidateIdField(item);

            dynamic clone = item;
            clone.Id = clone.Id ?? Guid.NewGuid().ToString("D");

            var db = ConnectionMultiplexer.GetDatabase();

            OnSaveData(db, clone.Id, item);
        }

        protected virtual void OnSaveData(IDatabase db, string id, T item)
        {
            db.StringSet(id, _serializer.Serialize(item));
        }

        private static void ValidateIdField(T item)
        {
            return;

            //if (item.Equals(default(T)))
            //    throw new ArgumentException(nameof(item));

            //var idField = typeof(T)
            //    .GetProperties(BindingFlags.Public)
            //    .Where(pi => pi.CanRead && pi.CanWrite)
            //    .FirstOrDefault(pi => pi.Name == "Id");

            //if (idField == null)
            //    throw new InvalidOperationException("T must contain a public string field named \"Id\"");
        }
    }
}