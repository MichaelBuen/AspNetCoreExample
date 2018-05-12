namespace AspNetCoreExample.Dal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    public interface IDdd : IDisposable
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
        ///  Renamed from Load to ReferenceTo.
        ///  The name Load gives the impression that it read the object from the database,
        ///  when in fact it just creates an object reference based on the id.
        /// </summary>
        /// <typeparam name="TDomainModel"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        TDomainModel ReferenceTo<TDomainModel>(object id) where TDomainModel : class;
        Task<TDomainModel> ReferenceToAsync<TDomainModel>(object id) where TDomainModel : class;
        // See the Persist notes at the bottom of this code.
        void Persist<TDomainModel>(TDomainModel transientObject) where TDomainModel : class;
        Task PersistAsync<TDomainModel>(TDomainModel transientObject) where TDomainModel : class;
        //void Flush();
        //Task FlushAsync();
        void Evict<TDomainModel>(TDomainModel obj) where TDomainModel : class;
        // void Attach<TDomainModel>(TDomainModel transientObject) where TDomainModel : class;

        /// <summary>
        /// Deletes obj and its children collections
        /// </summary>
        /// <typeparam name="TDomainModel"></typeparam>
        /// <param name="obj"></param>
        void DeleteAggregate<TDomainModel>(TDomainModel obj) where TDomainModel : class;
        Task DeleteAggregateAsync<TDomainModel>(TDomainModel obj) where TDomainModel : class;
        void SaveChanges();

        /// <summary>
        /// Clone's actual cloning is not implemented here. It's hard to make a generalized clone routine.
        /// Just follow this pattern: http://www.ienablemuch.com/2012/08/deep-copying-object-on-nhibernate-is.html
        /// </summary>
        /// <typeparam name="TDomainModel"></typeparam>
        /// <param name="transientObj"></param>
        /// <returns>New instance of transient object</returns>
        TDomainModel Clone<TDomainModel>(TDomainModel transientObj) where TDomainModel : class;

        void EagerlyLoad<TDomainModel>(TDomainModel model);

        void Commit();
        Task CommitAsync();

        void Rollback();
        Task RollbackAsync();


        // We can avoid Flush as long as we adhere to DDD that objects
        // relate to each other via object references instead of foreign key and id.
        // If we really need to obtain the generated id of an entity, 
        // do it after the transaction is committed.


        //void Flush();
        //Task FlushAsync();
    }

    /*

    For INSERT
    * Merge and Persist are both OK


    For UPDATE

    * Merge is ok for UPDATE if the detached object carries the full state of the entity, 
        as Merge doesn't need to read the database prior to updating. 
        No overhead of reading the database when updating, regardless of having second level cache or not.
        However, Merge is risky if the detached object doesn't carry the full state, as the previously written fields
        might be overwritten with blank or null values.
        Think of collection not being hydrated from detached object. It will result to collection being emptied
        when Merge is called.

    * Persist is safer when doing UPDATE especially partial UPDATE as there's less risk of 
        overwriting previously written fields. 
        Safe update is possible as the object is read first from database 
        before assigning the fields that are only needed to be updated. 
        
        To mitigate the overhead of fetching the object from the database prior to updating, use second-level cache.


    Due to risk associated with Merge, this Domain Access Layer exposes Persist only.

    */
}
