using System.Collections.Generic;
using System.Linq;

namespace Navislamia.Game.Extensions;

public static class ListExtensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable == null)
        {
            return true;
        }
        
        if (enumerable is ICollection<T> collection)
        {
            return collection.Count < 1;
        }
        return !enumerable.Any();
    }

    public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
    {
        if (collection == null)
        {
            return true;
        }
        return collection.Count < 1;
    }
}