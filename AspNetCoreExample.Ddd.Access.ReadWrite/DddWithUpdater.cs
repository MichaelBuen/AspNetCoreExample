namespace AspNetCoreExample.Ddd.Access.ReadWrite
{
    using System;
    using System.Threading.Tasks;
       
    using NHibernate;

    using AspNetCoreExample.Ddd.Access.Read;
       
    // not public, program to interface instead: http://www.ienablemuch.com/2014/10/var-abuse.html
    public class DddWithUpdater : Ddd, IDddWithUpdater
    {        
        protected bool _transactionCompleted;

        protected readonly ITransaction _transaction;

        public DddWithUpdater(ISessionFactory sessionFactory): base(sessionFactory)
            => _transaction = _session.BeginTransaction();

        void IDddWithUpdater.Persist<TDomainModel>(TDomainModel transientObject)
            => _session.Persist(transientObject);

        async Task IDddWithUpdater.PersistAsync<TDomainModel>(TDomainModel transientObject)
            => await _session.PersistAsync(transientObject);        
                                  
        void IDddWithUpdater.Evict<TDomainModel>(TDomainModel obj) => _session.Evict(obj);
              
        void IDddWithUpdater.DeleteAggregate<TDomainModel>(TDomainModel obj) => _session.Delete(obj);       

        async Task IDddWithUpdater.DeleteAggregateAsync<TDomainModel>(TDomainModel obj) 
            => await _session.DeleteAsync(obj);        

        void IDisposable.Dispose()
        {
            // http://www.hibernatingrhinos.com/products/nhprof/learn/alert/donotuseimplicittransactions

            if (_transactionCompleted)
                _transaction.Dispose();
                                              
            _session.Dispose();
        }


        TDomainModel IDddWithUpdater.Clone<TDomainModel>(TDomainModel transientObj) 
            => _session.Merge(transientObj);        
        
              
        void IDddWithUpdater.Commit()
        {                             
            // Removed the IsDirty check on Commit, CommitAsync, Rollback, RollbackAsync.
            // NHibernate can't detect dirtiness if the changes is made outside of NHibernate,
            // e.g., via Dapper. Also dirtiness can't be detected even 
            // from NHibernate 5's IQueryable DML.
            // e.g.,
            /*  
                await ddd.Query<Ddd.JobDomain.Profile>()
               .Where(x => x.UserFk == userId)
               .UpdateAsync(p => new Profile { AddressLine1 = dto.AddressLine1 }, cancellationToken);
            */

            //if (_session.IsDirty())
            //{   
            _transactionCompleted = true;
            _transaction.Commit();
            // }
        }


        async Task IDddWithUpdater.CommitAsync()
        {
            _transactionCompleted = true;
            await _transaction.CommitAsync();
        }

        void IDddWithUpdater.Rollback()
        {
            _transactionCompleted = true;
            _transaction.Rollback();
        }

        async Task IDddWithUpdater.RollbackAsync()
        {
            _transactionCompleted = true;
            await _transaction.RollbackAsync();
        }
    }

}

