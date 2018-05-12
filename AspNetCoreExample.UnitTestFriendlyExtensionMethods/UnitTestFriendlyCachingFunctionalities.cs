
using System.Linq;

using NHibernate.Linq;

namespace AspNetCoreExample
{
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