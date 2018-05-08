namespace AspNetCoreExample.Infrastructure.NHibernateInfra
{
    using System;
    using System.Data;
    using System.Data.Common;

    using NHibernate.UserTypes;
    using NHibernate.SqlTypes;
    using NHibernate.Engine;

    using NpgsqlTypes;


    [Serializable]
    public class JsonbType : IUserType
    {
        bool IUserType.IsMutable => false;

        Type IUserType.ReturnedType => typeof(Jsonb);

        SqlType[] IUserType.SqlTypes => 
            new SqlType[] 
            { 
                new NHibernateNpgsqlInfra.NpgsqlExtendedSqlType(DbType.Binary, NpgsqlDbType.Jsonb) 
            };

        object IUserType.Assemble(object cached, object owner) => cached;

        object IUserType.DeepCopy(object value) => value;

        object IUserType.Disassemble(object value) => value;

        bool IUserType.Equals(object x, object y)
        {
            if (x == null && y == null)
                return true;


            if (x == null || y == null)
                return false;

            bool areEqual = ((Jsonb)y).Equals(x);

            return areEqual;
        }

        int IUserType.GetHashCode(object x) => x == null ? 0 : x.GetHashCode();

        object IUserType.NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
        {
            if (names.Length != 1)
                throw new InvalidOperationException("Only expecting one column...");


            object value = rs[names[0]];

            if (value == DBNull.Value)
                return Jsonb.Null;
            else
                return Jsonb.Create((string)value);
        }

        void IUserType.NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            var parameter = (System.Data.Common.DbParameter)cmd.Parameters[index];

            if ((Jsonb)value == Jsonb.Null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.Value = value;
            }
        }

        object IUserType.Replace(object original, object target, object owner) => original;
    }
}
