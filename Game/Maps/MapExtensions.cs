using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.Maps
{
    public static class MapExtensions
    {
        public static void Populate<T>(this T[] array, Func<T> provider)
        {
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = provider();
            }
        }

        public static string GetStringContent(this string line, string header)
        {
            return line.StartsWith(header) ? line[header.Length..] : null;
        }
    }
}
