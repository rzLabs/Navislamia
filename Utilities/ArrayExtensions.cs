using System;

namespace Navislamia.Utilities;

public static class ArrayExtensions
{
    public static void Populate<T>(this T[] array, Func<T> provider)
    {
        for (var i = 0; i < array.Length; i++)
        {
            array[i] = provider();
        }
    }
}