namespace AspNetCoreExample.Ddd
{
    using System.Collections.Generic;

    static class DddHelper
    {
        internal static ICollection<T> AsCollection<T>(this IEnumerable<T> enumerable) => (ICollection<T>)enumerable;
    }
}
