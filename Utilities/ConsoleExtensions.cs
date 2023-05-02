using System;

namespace Navislamia.Utilities;

public static class ConsoleExtensions
{
    public static void ClearLastLine()
    {
        Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - 1);
        Console.Write("\b");
    }
}