namespace AspNetCoreExample.Infrastructure.NHibernateNpgsqlInfra
{
    using System.Data.Common;

    using NHibernate.SqlTypes;
    using Npgsql;

    public class NpgsqlDriverExtended : NHibernate.Driver.NpgsqlDriver
    {        
        protected override void InitializeParameter(DbParameter dbParam, string name, SqlType sqlType)
        {            
            if (sqlType is NpgsqlExtendedSqlType && dbParam is NpgsqlParameter)
            {
                this.InitializeParameter(dbParam as NpgsqlParameter, name, sqlType as NpgsqlExtendedSqlType);
            }
            else
            {
                base.InitializeParameter(dbParam, name, sqlType);
            }
        }

        protected virtual void InitializeParameter(NpgsqlParameter dbParam, string name, NpgsqlExtendedSqlType sqlType)
        {
            if (sqlType == null)
            {
                throw new NHibernate.QueryException(string.Format("No type assigned to parameter '{0}'", name));
            }

            dbParam.ParameterName = FormatNameForParameter(name);
            dbParam.DbType        = sqlType.DbType;
            dbParam.NpgsqlDbType  = sqlType.NpgDbType;
        }

    }// class
}
