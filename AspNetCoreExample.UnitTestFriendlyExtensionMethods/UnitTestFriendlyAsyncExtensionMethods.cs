namespace AspNetCoreExample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using NHibernate.Linq;


    public static class UnitTestFriendlyAsyncExtensionMethods
    {
        // TODO: Create unit-test-friendly wrappers for all async Linq extension methods

        public static async Task<int> CountAsyncOk<T>(
            this IQueryable<T> query,
            CancellationToken cancellationToken = default)
            =>
            query.Provider.GetType() == typeof(NHibernate.Linq.DefaultQueryProvider) ?
                await LinqExtensionMethods.CountAsync(query, cancellationToken)
            :
                query.Count();


        public static async Task<T> SingleOrDefaultAsyncOk<T>(
            this IQueryable<T> query,
            CancellationToken cancellationToken = default)
            =>
            query.Provider.GetType() == typeof(NHibernate.Linq.DefaultQueryProvider) ?
                await LinqExtensionMethods.SingleOrDefaultAsync(query, cancellationToken)
            :
                query.SingleOrDefault();


        public static async Task<T> SingleOrDefaultAsyncOk<T>(
            this IQueryable<T> query,
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
            =>
            query.Provider.GetType() == typeof(NHibernate.Linq.DefaultQueryProvider) ?
                await LinqExtensionMethods.SingleOrDefaultAsync(query, predicate, cancellationToken)
            :
                query.SingleOrDefault(predicate);


        public static async Task<List<T>> ToListAsyncOk<T>(
            this IQueryable<T> query,
            CancellationToken cancellationToken = default)
            =>
            query.Provider.GetType() == typeof(NHibernate.Linq.DefaultQueryProvider) ?
                await LinqExtensionMethods.ToListAsync(query, cancellationToken)
            :
                query.ToList();

        public static async Task<bool> AnyAsyncOk<T>(
            this IQueryable<T> query,
            CancellationToken cancellationToken = default)
            =>
            query.Provider.GetType() == typeof(NHibernate.Linq.DefaultQueryProvider) ?
                await LinqExtensionMethods.AnyAsync(query, cancellationToken)
            :
                query.Any();

        public static async Task<bool> AnyAsyncOk<T>(
            this IQueryable<T> query,
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
            =>
            query.Provider.GetType() == typeof(NHibernate.Linq.DefaultQueryProvider) ?
                await LinqExtensionMethods.AnyAsync(query, predicate, cancellationToken)
            :
                query.Any(predicate);
    }
}
