namespace AspNetCoreExample.Infrastructure.NHibernateNpgsqlInfra
{
    using NpgsqlTypes;
    using System.Data;

    public class NpgsqlExtendedSqlType : NHibernate.SqlTypes.SqlType
    {
        readonly NpgsqlDbType npgDbType;

        public NpgsqlExtendedSqlType(DbType dbType, NpgsqlDbType npgDbType) 
            : base(dbType) => this.npgDbType = npgDbType;
    
        public NpgsqlExtendedSqlType(DbType dbType, NpgsqlDbType npgDbType, int length) 
            : base(dbType, length) => this.npgDbType = npgDbType;

        public NpgsqlExtendedSqlType(DbType dbType, NpgsqlDbType npgDbType, byte precision, byte scale) 
            : base(dbType, precision, scale) => this.npgDbType = npgDbType;
        
        public NpgsqlDbType NpgDbType => this.npgDbType;
    }
}
