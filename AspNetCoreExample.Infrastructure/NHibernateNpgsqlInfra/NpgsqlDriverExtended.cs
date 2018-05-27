namespace AspNetCoreExample.Infrastructure.NHibernateNpgsqlInfra
{
	using System.Data.Common;

    using NHibernate.SqlTypes;
    using Npgsql;

    public class NpgsqlDriverExtended : NHibernate.Driver.NpgsqlDriver
    {
        // this gets called when an SQL is executed
        protected override void InitializeParameter(DbParameter dbParam, string name, SqlType sqlType)
        {
            if (sqlType is NpgsqlExtendedSqlType && dbParam is NpgsqlParameter)
            {
                this.InitializeParameter(dbParam as NpgsqlParameter, name, sqlType as NpgsqlExtendedSqlType);
            }
            else
            {
                base.InitializeParameter(dbParam, name, sqlType);

				// Solved this problem: https://github.com/nhibernate/nhibernate-core/issues/1708
                if (sqlType.DbType == System.Data.DbType.DateTime)
                {
                    dbParam.DbType = System.Data.DbType.DateTimeOffset;
                }
            }
        }

        // NpgsqlExtendedSqlType is used for Jsonb
        protected virtual void InitializeParameter(NpgsqlParameter dbParam, string name, NpgsqlExtendedSqlType sqlType)
        {
            if (sqlType == null)
            {
                throw new NHibernate.QueryException(string.Format("No type assigned to parameter '{0}'", name));
            }

            dbParam.ParameterName = FormatNameForParameter(name);
            dbParam.DbType = sqlType.DbType;
            dbParam.NpgsqlDbType = sqlType.NpgDbType;
        }
    }
}
