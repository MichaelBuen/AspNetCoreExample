// Persistence concerns goes here.

// For access concerns, go to AspNetCoreExample.Ddd.Accessor's IDdd interface

namespace AspNetCoreExample.Ddd.Access.ReadWrite
{
    using System;
    using System.Threading.Tasks;

    using AspNetCoreExample.Ddd.Access.Read;

    public interface IDddWithUpdater: IDdd, IDisposable
    {
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
        
        /// <summary>
        /// Clone's actual cloning is not implemented here. It's hard to make a generalized clone routine.
        /// Just follow this pattern: http://www.ienablemuch.com/2012/08/deep-copying-object-on-nhibernate-is.html
        /// </summary>
        /// <typeparam name="TDomainModel"></typeparam>
        /// <param name="transientObj"></param>
        /// <returns>New instance of transient object</returns>
        TDomainModel Clone<TDomainModel>(TDomainModel transientObj) where TDomainModel : class;
                
        void Commit();
        Task CommitAsync();

        void Rollback();
        Task RollbackAsync();


        // We can avoid Flush as long as we adhere to DDD principle that objects
        // relate to each other via object references instead of foreign key Ids.
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
