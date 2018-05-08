using System;
using System.Linq;

using NHibernate;

using Dapper;

using System.Threading.Tasks;
using System.Collections.Generic;

namespace AspNetCoreExample.Dal
{


    // not public, program to interface instead: http://www.ienablemuch.com/2014/10/var-abuse.html
    class Ddd : IDdd
    {
        ISession _session;
        ITransaction _transaction;

        bool _withTransaction;
        bool _transactionCompleted;


        internal Ddd(NHibernate.ISessionFactory sessionFactory) 
            : this(sessionFactory, withTransaction: false)
        {                        
        }


        internal Ddd(ISessionFactory sessionFactory, bool withTransaction) 
        {
            _session = sessionFactory.OpenSession();
            _withTransaction = withTransaction;

            if (_withTransaction)
                _transaction = _session.BeginTransaction();

        }

        IQueryable<TDomainModel> IDdd.Query<TDomainModel>()
        {            
            return _session.Query<TDomainModel>();
        }


        IQueryOver<TDomainModel> IDdd.QueryOver<TDomainModel>()
        {            
            return _session.QueryOver<TDomainModel>();
        }

        // http://stackoverflow.com/questions/5683111/warning-from-explicitly-implementing-an-interface-with-optional-paramters
        IEnumerable<TDto> IDdd.QueryToDto<TDto>(string sql, object param)
        {
            // Dapper's Query extension method
            return _session.Connection.Query<TDto>(sql, param, transaction: null, buffered: true, commandTimeout: null, commandType: null);
        }



        TDomainModel IDdd.Get<TDomainModel>(object id)
        {            
            return _session.Get<TDomainModel>(id);
        }

        async Task<TDomainModel> IDdd.GetAsync<TDomainModel>(object id)
        {            
            return await _session.GetAsync<TDomainModel>(id);
        }

        TDomainModel IDdd.JustKey<TDomainModel>(object id)
        {
            if (id == null)
                return default(TDomainModel);
            else
                return _session.Load<TDomainModel>(id);
        }



        void IDdd.Persist<TDomainModel>(TDomainModel transientObject)
        {                                        
            _session.Persist(transientObject);
        }

        async Task IDdd.PersistAsync<TDomainModel>(TDomainModel transientObject)
        {
            await _session.PersistAsync(transientObject);
        }


        void IDdd.Evict<TDomainModel>(TDomainModel obj)
        {
            _session.Evict(obj);            
        }


        void IDdd.DeleteAggregate<TDomainModel>(TDomainModel obj)
        {
            _session.Delete(obj);
        }

        async Task IDdd.DeleteAggregateAsync<TDomainModel>(TDomainModel obj)
        {
            await _session.DeleteAsync(obj);
        }
        

        void IDisposable.Dispose()
        {
            // http://www.hibernatingrhinos.com/products/nhprof/learn/alert/donotuseimplicittransactions

            if (_withTransaction)
            {         
                if (_transactionCompleted)                    
                    _transaction.Dispose();
            }

            
            _session.Dispose();
        }

        
        TDomainModel IDdd.Clone<TDomainModel>(TDomainModel transientObj)
        {
            return _session.Merge(transientObj);
        }

        void IDdd.SaveChanges()
        {
            if (!_withTransaction)
            {
                throw new AspNetCoreExampleException("Developer error. Cannot save changes if access is not opened for changes. Use OpenAccessForChanges().");
            }

            _transactionCompleted = true;
        }

        void IDdd.EagerlyLoad<TDomainModel>(TDomainModel model)
        {
            NHibernateUtil.Initialize(model);
        }

        void IDdd.Commit()
        {
            if (!_withTransaction)
            {
				throw new AspNetCoreExampleException("Developer error. Cannot save changes if access is not opened for changes. Use OpenAccessForChanges().");
            }


            if (_session.IsDirty())
            {   
                _transactionCompleted = true;
                _transaction.Commit();
            }
        }


        async Task IDdd.CommitAsync()
        {
            if (!_withTransaction)
            {
				throw new AspNetCoreExampleException("Developer error. Cannot save changes if access is not opened for changes. Use OpenAccessForChanges().");
            }


            if (await _session.IsDirtyAsync())
            {
                _transactionCompleted = true;
                await _transaction.CommitAsync();
            }
        }

        void IDdd.Rollback()
        {
            if (!_withTransaction)
            {
				throw new AspNetCoreExampleException("Developer error. Cannot save changes if access is not opened for changes. Use OpenAccessForChanges().");
            }


            if (_session.IsDirty())
            {
                _transactionCompleted = true;
                _transaction.Rollback();
            }
        }

        async Task IDdd.RollbackAsync()
        {
            if (!_withTransaction)
            {
				throw new AspNetCoreExampleException("Developer error. Cannot save changes if access is not opened for changes. Use OpenAccessForChanges().");
            }


            if (await _session.IsDirtyAsync())
            {
                _transactionCompleted = true;
                await _transaction.RollbackAsync();
            }
        }
    }
    
}

