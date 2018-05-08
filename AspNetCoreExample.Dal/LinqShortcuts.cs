using System;
using System.Linq;
using System.Linq.Expressions;

namespace AspNetCoreExample
{
    public static class LinqShortcuts
    {
        public static IQueryable<T> LimitToPage<T>(
            this IQueryable<T> q,
            int pageNumber, int pageLimit
        ) => q.Skip((pageNumber - 1) * pageLimit).Take(pageLimit);

        // There's no shame on copy-pasting, especially if it comes from high-quality Q&A site like stackoverflow
        // or from Jon Skeet :)
        // http://stackoverflow.com/questions/388708/ascending-descending-in-linq-can-one-change-the-order-via-parameter
        public static IOrderedQueryable<TSource> OrderByWithDirection<TSource, TKey>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector,
            bool ascending
        ) => ascending ? source.OrderBy(keySelector) : source.OrderByDescending(keySelector);
    }
}
