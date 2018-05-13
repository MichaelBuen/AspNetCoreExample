namespace AspNetCoreExample.Ddd.Access.Read
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Dapper;
	using NHibernate;
    	  
	public class Ddd: IDdd
    {
		protected readonly ISession _session;
               
		public Ddd(ISessionFactory sessionFactory) => _session = sessionFactory.OpenSession();            
        
        IQueryable<TDomainModel> IDdd.Query<TDomainModel>() => _session.Query<TDomainModel>();

        IQueryOver<TDomainModel> IDdd.QueryOver<TDomainModel>() => _session.QueryOver<TDomainModel>();

        // http://stackoverflow.com/questions/5683111/warning-from-explicitly-implementing-an-interface-with-optional-paramters
        IEnumerable<TDto> IDdd.QueryToDto<TDto>(string sql, object param)
        {
            // Dapper's Query extension method
            return _session.Connection.Query<TDto>(
                sql,
                param,
                transaction: null,
                buffered: true,
                commandTimeout: null,
                commandType: null
            );
        }

        TDomainModel IDdd.Get<TDomainModel>(object id) => _session.Get<TDomainModel>(id);

        async Task<TDomainModel> IDdd.GetAsync<TDomainModel>(object id) => await _session.GetAsync<TDomainModel>(id);


        TDomainModel IDdd.Map<TDomainModel>(object id)
        {
            if (id == null)
                return default(TDomainModel);
            else
                return _session.Load<TDomainModel>(id);
        }

        async Task<TDomainModel> IDdd.MapAsync<TDomainModel>(object id)
        {
            if (id == null)
                return default(TDomainModel);
            else
                return await _session.LoadAsync<TDomainModel>(id);
        }

		void IDdd.EagerlyLoad<TDomainModel>(TDomainModel model) 
            => NHibernateUtil.Initialize(model);        
        
		void IDisposable.Dispose() => _session.Dispose();
    }
}
