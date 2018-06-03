namespace AspNetCoreExample.Infrastructure.NHibernateInfra
{
    [System.Serializable]
    public class CustomUtcType : NHibernate.UserTypes.IUserType
    {
        bool NHibernate.UserTypes.IUserType.IsMutable => false;

        System.Type NHibernate.UserTypes.IUserType.ReturnedType => typeof(System.DateTime);

        NHibernate.SqlTypes.SqlType[] NHibernate.UserTypes.IUserType.SqlTypes => 
            new [] { new NHibernate.SqlTypes.SqlType(System.Data.DbType.DateTime) };

        object NHibernate.UserTypes.IUserType.Assemble(object cached, object owner) => cached;

        object NHibernate.UserTypes.IUserType.DeepCopy(object value) => value;

        object NHibernate.UserTypes.IUserType.Disassemble(object value) => value;

        object NHibernate.UserTypes.IUserType.Replace(object original, object target, object owner) => original;

        int NHibernate.UserTypes.IUserType.GetHashCode(object x) => x == null ? 0 : x.GetHashCode();

        void NHibernate.UserTypes.IUserType.NullSafeSet(
            System.Data.Common.DbCommand cmd, 
            object value, 
            int index,
            NHibernate.Engine.ISessionImplementor session
        ) 
        => cmd.Parameters[index].Value = value != null ? value: System.DBNull.Value;

        bool NHibernate.UserTypes.IUserType.Equals(object x, object y)
        =>
            x == null && y == null ?
                true
            : x == null || y == null ?
                false
            :
                ((System.DateTime)y).Equals(x);
        

        object NHibernate.UserTypes.IUserType.NullSafeGet(
            System.Data.Common.DbDataReader rs, string[] names,
            NHibernate.Engine.ISessionImplementor session, 
            object owner
        )
        =>
            names.Length != 1 ? 
                throw new System.InvalidOperationException("Only expecting one column...")
            : rs[names[0]] is System.DateTime value ? 
                value.ToUniversalTime() 
            : 
                (System.DateTime?) null;        
    }
}
