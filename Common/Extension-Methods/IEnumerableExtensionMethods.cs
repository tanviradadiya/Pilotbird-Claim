using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public static class IEnumerableExtensionMethods
    {
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> setToCheck)
            => setToCheck ?? Enumerable.Empty<T>();
    }
}
