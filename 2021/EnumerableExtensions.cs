using System.Collections.Generic;
using System.Linq;

namespace _2021;

public static class EnumerableExtensions
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> items)
    {
        return items?.Select((item, index) => (item, index)) ?? Enumerable.Empty<(T item, int index)>();
    }
}