namespace AspNetCoreExample
{
    using System.Linq;

    using NHibernate.Linq;

    public static class UnitTestFriendlyExtensionMethods
    {
        public static IQueryable<T> CacheableOk<T>(this IQueryable<T> query)
        {
            if (query.Provider.GetType() == typeof(NHibernate.Linq.DefaultQueryProvider))
                query = LinqExtensionMethods.WithOptions(
                    query,
                    o =>
                    {
                        o.SetCacheable(true);
                    });

            return query;
        }
    }
}