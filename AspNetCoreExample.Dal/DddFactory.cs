namespace AspNetCoreExample.Dal
{
    public class DddFactory : IDddFactory
    {
        NHibernate.ISessionFactory _sessionFactory;

        public DddFactory(NHibernate.ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        IDdd IDddFactory.OpenDdd()
        {
            return new Ddd(_sessionFactory);
        }

        IDdd IDddFactory.OpenDddForChanges()
        {
            return new Ddd(_sessionFactory, withTransaction: true);
        }
    }
}
