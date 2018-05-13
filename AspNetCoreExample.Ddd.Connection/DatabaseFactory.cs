namespace AspNetCoreExample.Ddd.Connection
{
	using AspNetCoreExample.Ddd.Access.Read;
	using AspNetCoreExample.Ddd.Access.ReadWrite;

	public class DatabaseFactory : IDatabaseFactory
    {
		readonly NHibernate.ISessionFactory _sessionFactory;
        
        public DatabaseFactory(NHibernate.ISessionFactory sessionFactory) => _sessionFactory = sessionFactory;
               
		IDdd IDatabaseFactory.OpenDdd() => new Ddd(_sessionFactory);

		IDddWithUpdater IDatabaseFactory.OpenDddForUpdate() => new DddWithUpdater(_sessionFactory);  
    }
}
