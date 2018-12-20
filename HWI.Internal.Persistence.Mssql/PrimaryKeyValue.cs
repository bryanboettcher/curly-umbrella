using System;
using System.Data;

namespace HWI.Internal.Persistence.Mssql
{
    public class PrimaryKeyValue<TPoco> : IEquatable<PrimaryKeyValue<TPoco>> where TPoco : class
    {
        public string ColumnName { get; }

        public SqlDbType DataType { get; }

        public Func<TPoco, object> Selector { get; }

        public PrimaryKeyValue(string columnName, SqlDbType dataType, Func<TPoco, object> selector)
        {
            ColumnName = columnName;
            DataType = dataType;
            Selector = selector;
        }

        public bool Equals(PrimaryKeyValue<TPoco> other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (Equals(other.ColumnName, ColumnName))
                return Equals(other.DataType, DataType);
            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() == typeof(PrimaryKeyValue<TPoco>))
                return Equals((PrimaryKeyValue<TPoco>)obj);
            return false;
        }

        public override int GetHashCode()
        {
            return (ColumnName != null ? ColumnName.GetHashCode() : 0) * 397 ^ DataType.GetHashCode();
        }

        public static bool operator ==(PrimaryKeyValue<TPoco> left, PrimaryKeyValue<TPoco> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PrimaryKeyValue<TPoco> left, PrimaryKeyValue<TPoco> right)
        {
            return !Equals(left, right);
        }
    }
}