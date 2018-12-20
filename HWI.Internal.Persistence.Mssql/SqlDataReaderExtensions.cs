using System;
using System.Data.SqlClient;

namespace HWI.Internal.Persistence.Mssql
{
    public static class SqlDataReaderExtensions
    {
        public static T Get<T>(this SqlDataReader reader, string columnName, T def = default(T))
        {
            var ordinal = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(ordinal)) return def;

            var columnType = typeof(T);

            if (columnType == typeof(string))
                return (T)(object)reader.GetString(ordinal);

            if (columnType == typeof(int))
                return (T)(object)reader.GetInt32(ordinal);

            if (columnType == typeof(long))
                return (T)(object)reader.GetInt64(ordinal);

            if (columnType == typeof(bool))
                return (T)(object)reader.GetBoolean(ordinal);

            if (columnType == typeof(DateTime))
                return (T)(object)reader.GetDateTime(ordinal);

            if (columnType == typeof(short))
                return (T)(object)reader.GetInt16(ordinal);

            if (columnType == typeof(byte))
                return (T)(object)reader.GetByte(ordinal);
            
            if (columnType == typeof(decimal))
                return (T)(object)reader.GetDecimal(ordinal);

            if (columnType == typeof(double))
                return (T)(object)reader.GetDouble(ordinal);

            if (columnType == typeof(float))
                return (T)(object)reader.GetFloat(ordinal);

            if (columnType == typeof(Guid))
                return (T)(object)reader.GetGuid(ordinal);
            
            if (columnType == typeof(char))
                return (T)(object)reader.GetChar(ordinal);
            

            throw new ArgumentException($"Cannot automatically translate from {columnType}", nameof(T));
        }
    }
}