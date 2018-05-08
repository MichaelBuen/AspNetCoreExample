using System;
using System.Collections.Generic;

namespace AspNetCoreExample.Ddd
{
    static class DddHelper
    {        
        internal static IList<T> AsList<T>(this IEnumerable<T> enumerable) => (IList<T>)enumerable;
    }
}
