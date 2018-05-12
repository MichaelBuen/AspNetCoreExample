using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace AspNetCoreExample
{
    public static class UnitTestFriendlyAsyncExtensionMethods
    {
		// TODO: Create unit-test-friendly wrappers for all async Linq extension methods


		public static async Task<int> CountAsyncOk<T>(
			this IQueryable<T> query, 
			CancellationToken cancellationToken = default)
		{
			if (query.Provider.GetType() == typeof(NHibernate.Linq.DefaultQueryProvider))
			{
				return await LinqExtensionMethods.CountAsync(query, cancellationToken);
			}
			else 
			{
				return query.Count();
			}

		}

		public static async Task<T> SingleOrDefaultAsyncOk<T>(
            this IQueryable<T> query,
            CancellationToken cancellationToken = default)
        {
            if (query.Provider.GetType() == typeof(NHibernate.Linq.DefaultQueryProvider))
            {
                return await LinqExtensionMethods.SingleOrDefaultAsync(query, cancellationToken);
            }
            else
            {
				return query.SingleOrDefault();
            }

        }


		public static async Task<T> SingleOrDefaultAsyncOk<T>(
            this IQueryable<T> query,
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            if (query.Provider.GetType() == typeof(NHibernate.Linq.DefaultQueryProvider))
            {
                return await LinqExtensionMethods.SingleOrDefaultAsync(query, predicate, cancellationToken);
            }
            else
            {
                return query.SingleOrDefault(predicate);
            }

        }
    }
}
