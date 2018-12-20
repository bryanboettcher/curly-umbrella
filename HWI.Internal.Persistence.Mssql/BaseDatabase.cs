using System;

namespace HWI.Internal.Persistence.Mssql
{
    public abstract class BaseDatabase
    {
        public static BaseDatabase GetDatabase<TDatabase>() where TDatabase : BaseDatabase
        {
            return Activator.CreateInstance<TDatabase>();
        }

        public abstract string GetConnectionString();
    }
}
