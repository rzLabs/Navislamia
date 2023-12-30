using System.Collections.Generic;
using System.Linq;

namespace Navislamia.Game.Extensions;

public static class LinqExtensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
    {
        return enumerable switch
        {
            null => true,
            ICollection<T> collection => collection.Count < 1,
            _ => !enumerable.Any()
        };
    }
}