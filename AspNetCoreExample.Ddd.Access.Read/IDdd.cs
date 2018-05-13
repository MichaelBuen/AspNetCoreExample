namespace AspNetCoreExample.Ddd.Access.Read
{
	using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


	public interface IDdd: IDisposable
    {
        /// <summary>
        /// Query must not be used on application layer (e.g., MVC, Web API).
        /// This must be used only on domain model. The business logic, e.g., queries must be done on domain model.
        /// </summary>
        /// <typeparam name="TDomainModel"></typeparam>
        /// <returns></returns>
        IQueryable<TDomainModel> Query<TDomainModel>();

        /// <summary>
        /// QueryOver must not be used on application layer (e.g., MVC, Web API).
        /// This must be used only on domain model. The business logic, e.g., queries must be done on domain model.
        /// </summary>
        /// <typeparam name="TDomainModel"></typeparam>
        /// <returns></returns>
        NHibernate.IQueryOver<TDomainModel> QueryOver<TDomainModel>() where TDomainModel : class;


        /// <summary>
        /// QueryToDto must not be used on application layer (e.g., MVC, Web API).
        /// This must be used only on domain model. The business logic, e.g., queries must be done on domain model.
        /// </summary>
        /// <typeparam name="TDomainModel"></typeparam>
        /// <returns></returns>
        IEnumerable<TDto> QueryToDto<TDto>(string sql, object param = null);

        /// <summary>
        /// NHibernate gets the object from memory if second-level cache is used.
        /// If not, it gets it from database.
        /// </summary>
        /// <typeparam name="TDomainModel"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        TDomainModel Get<TDomainModel>(object id);

        Task<TDomainModel> GetAsync<TDomainModel>(object id);

        /// <summary>
        /// Renamed from Load to Map.
        /// The name Load gives the impression that it always read the object from the database,
		/// when in fact most of the time, it is just used for mapping
		/// an identity/foreignkey to an object.
        /// </summary>
        /// <typeparam name="TDomainModel"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        TDomainModel Map<TDomainModel>(object id) where TDomainModel : class;
        Task<TDomainModel> MapAsync<TDomainModel>(object id) where TDomainModel : class;

		// http://nhibernate.info/doc/howto/various/lazy-loading-eager-loading.html
        void EagerlyLoad<TDomainModel>(TDomainModel model);
    }       
}
