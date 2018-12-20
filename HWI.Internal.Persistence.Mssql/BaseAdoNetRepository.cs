using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace HWI.Internal.Persistence.Mssql
{
    public abstract class BaseAdoNetRepository<TDatabase, TPoco> where TDatabase : BaseDatabase, new() where TPoco : class
    {
        private readonly object _locker = new object();
        private readonly ICollection<TPoco> _pendingCreatedItems = new HashSet<TPoco>();
        private readonly ICollection<TPoco> _pendingUpdatedItems = new HashSet<TPoco>();
        private readonly ICollection<TPoco> _pendingDeletedItems = new HashSet<TPoco>();
        private readonly IDictionary<string, SqlDbType> _paramTypes = new Dictionary<string, SqlDbType>();
        private readonly IDictionary<string, Func<TPoco, object>> _paramValues = new Dictionary<string, Func<TPoco, object>>();

        private readonly IDictionary<Type, SqlDbType> _sqlDbTypes = new Dictionary<Type, SqlDbType>
        {
            {typeof(long), SqlDbType.BigInt},
            {typeof(int), SqlDbType.Int},
            {typeof(short), SqlDbType.SmallInt},
            {typeof(byte), SqlDbType.TinyInt},
            {typeof(bool), SqlDbType.Bit},
            {typeof(decimal), SqlDbType.Decimal},
            {typeof(double), SqlDbType.Float},
            {typeof(float), SqlDbType.Real},
            {typeof(Guid), SqlDbType.UniqueIdentifier},
            {typeof(DateTime), SqlDbType.DateTime2},
            {typeof(char), SqlDbType.Char},

            // nullables
            {typeof(long?), SqlDbType.BigInt},
            {typeof(int?), SqlDbType.Int},
            {typeof(short?), SqlDbType.SmallInt},
            {typeof(byte?), SqlDbType.TinyInt},
            {typeof(bool?), SqlDbType.Bit},
            {typeof(decimal?), SqlDbType.Decimal},
            {typeof(double?), SqlDbType.Float},
            {typeof(float?), SqlDbType.Real},
            {typeof(Guid?), SqlDbType.UniqueIdentifier},
            {typeof(DateTime?), SqlDbType.DateTime2},
            {typeof(char?), SqlDbType.Char},
            {typeof(string), SqlDbType.VarChar},
            {typeof(byte[]), SqlDbType.VarBinary}
        };

        private readonly List<PrimaryKeyValue<TPoco>> _primaryKeys = new List<PrimaryKeyValue<TPoco>>();
        private string _createSql;
        private string _updateSql;
        private string _deleteSql;

        protected virtual SqlCommand GetSqlCommand(string sql)
        {
            return GetSqlCommand(GetSqlConnection, sql);
        }

        protected virtual SqlCommand GetSqlCommand(Func<SqlConnection> connection, string sql)
        {
            var sqlCommand = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = sql,
                Connection = connection()
            };

            if (sqlCommand.Connection.State != ConnectionState.Open)
                sqlCommand.Connection.Open();

            return sqlCommand;
        }

        protected virtual SqlConnection GetSqlConnection()
        {
            var sqlConnection = new SqlConnection
            {
                ConnectionString = Activator.CreateInstance<TDatabase>().GetConnectionString()
            };

            sqlConnection.Open();

            return sqlConnection;
        }


        protected virtual IEnumerable<TResult> GetSqlResults<TResult>(SqlCommand cmd, Func<SqlDataReader, TResult> getData)
        {
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                        yield return getData(reader);
                }
            }

            cmd.Connection.Close();
        }


        public virtual void Create(TPoco existing)
        {
            lock (_locker)
            {
                _pendingCreatedItems.Add(existing);
            }
        }


        public virtual void Save()
        {
            lock (_locker)
            {
                CreateItems();
                UpdateItems();
                DeleteItems();
            }
        }

        protected virtual void CreateItems()
        {
            if (!_pendingCreatedItems.Any())
                return;

            using (var sqlCommand = GetSqlCommand(_createSql))
            {
                AddParameters(sqlCommand);
                foreach (var primaryKey in _primaryKeys)
                    sqlCommand.Parameters.RemoveAt(primaryKey.ColumnName);

                foreach (var pendingCreatedItem in _pendingCreatedItems)
                    PersistValuesToDatabase(sqlCommand, pendingCreatedItem, true);

                sqlCommand.Connection.Close();
            }
            _pendingCreatedItems.Clear();
        }

        protected virtual void UpdateItems()
        {
            if (!_pendingUpdatedItems.Any())
                return;

            using (var sqlCommand = GetSqlCommand(_updateSql))
            {
                AddParameters(sqlCommand);
                foreach (var pendingUpdatedItem in _pendingUpdatedItems)
                    PersistValuesToDatabase(sqlCommand, pendingUpdatedItem, false);

                sqlCommand.Connection.Close();
            }

            _pendingUpdatedItems.Clear();
        }

        protected virtual void DeleteItems()
        {
            if (!_pendingDeletedItems.Any())
                return;

            using (var sqlCommand = GetSqlCommand(_deleteSql))
            {
                foreach (var primaryKey in _primaryKeys)
                    sqlCommand.Parameters.Add(primaryKey.ColumnName, primaryKey.DataType);

                foreach (var pendingDeletedItem in _pendingDeletedItems)
                {
                    foreach (var primaryKey in _primaryKeys)
                        sqlCommand.Parameters[primaryKey.ColumnName].Value = primaryKey.Selector(pendingDeletedItem);

                    sqlCommand.ExecuteNonQuery();
                }

                sqlCommand.Connection.Close();
            }

            _pendingDeletedItems.Clear();
        }


        public virtual bool Delete(TPoco existing)
        {
            if (_pendingCreatedItems.Contains(existing))
                return _pendingCreatedItems.Remove(existing);

            if (_pendingUpdatedItems.Contains(existing))
                _pendingUpdatedItems.Remove(existing);

            _pendingDeletedItems.Add(existing);
            return true;
        }


        public virtual void Update(TPoco existing)
        {
            if (_pendingDeletedItems.Contains(existing))
                return;

            _pendingUpdatedItems.Add(existing);
        }


        protected void CreateSqlIs(string createSql)
        {
            if (!string.IsNullOrEmpty(_createSql))
                throw new ReadOnlyException("Cannot change sql after it has been set");

            _createSql = createSql;
        }


        protected void UpdateSqlIs(string updateSql)
        {
            if (!string.IsNullOrEmpty(_updateSql))
                throw new ReadOnlyException("Cannot change sql after it has been set");

            _updateSql = updateSql;
        }


        protected void DeleteSqlIs(string deleteSql)
        {
            if (!string.IsNullOrEmpty(_deleteSql))
                throw new ReadOnlyException("Cannot change sql after it has been set");

            _deleteSql = deleteSql;
        }


        protected virtual TPoco GetDataFromReader(SqlDataReader reader)
        {
            throw new NotImplementedException();
        }

        protected virtual void AddParameters(SqlCommand cmd)
        {
            cmd.Parameters.AddRange(_paramTypes.Select(p => SqlParameter(p.Key, p.Value)).ToArray());
        }

        protected virtual SqlParameter SqlParameter(string name, SqlDbType dataType)
        {
            return new SqlParameter(name, dataType);
        }

        protected virtual void PopulateParametersWithValues(SqlCommand cmd, TPoco existing, bool removePrimaryKey)
        {
            using (var enumerator = _paramValues.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var pv = enumerator.Current;
                    var obj = pv.Value(existing);
                    if (!removePrimaryKey || !_primaryKeys.Exists(x => x.ColumnName == pv.Key))
                        cmd.Parameters[pv.Key].Value = GetSanitizedValue(obj);
                }
            }
        }

        protected virtual object GetSanitizedValue(object value)
        {
            if (value is ValueType)
                return value;
            return value ?? DBNull.Value;
        }

        protected virtual void PersistValuesToDatabase(SqlCommand cmd, TPoco existing, bool useCreateBehavior)
        {
            PopulateParametersWithValues(cmd, existing, useCreateBehavior);
            cmd.ExecuteNonQuery();
        }


        protected virtual void AssignParameter<TType>(string paramName, Func<TPoco, TType> accessor, bool isPrimaryKey = false, SqlDbType sqlDbType = SqlDbType.BigInt)
        {
            if (sqlDbType == SqlDbType.BigInt)
                sqlDbType = GetSqlDbType<TType>();

            object ParamValue(TPoco t) => accessor(t);

            _paramTypes[paramName] = sqlDbType;
            _paramValues[paramName] = ParamValue;

            if (!isPrimaryKey)
                return;

            AssignPrimaryKey(paramName, sqlDbType, ParamValue);
        }

        protected virtual void AssignPrimaryKey(string paramName, SqlDbType sqlDbType, Func<TPoco, object> paramValue)
        {
            if (_primaryKeys.Exists(x => x.ColumnName == paramName && x.DataType == sqlDbType))
                return;

            _primaryKeys.Add(new PrimaryKeyValue<TPoco>(paramName, sqlDbType, paramValue));
        }

        protected virtual SqlDbType GetSqlDbType<TType>()
        {
            var key = typeof(TType);

            if (!_sqlDbTypes.ContainsKey(key))
                throw new DataException("No SQL data-type mapping found for " + key.Name);

            return _sqlDbTypes[key];
        }
    }
}