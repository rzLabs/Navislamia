using System;

namespace Navislamia.Game.Scripting;

public static class ScriptExtensions
{

    /// <summary>
    /// Convert provided decorated string into user friendly view 
    /// </summary>
    /// <seealso cref="MoonSharp.Interpreter.InterpreterException"/>
    /// <param name="decoratedMessage">Decorated Moonsharp exception message</param>
    /// <returns>String containing relevant information to the exception</returns>
    public static string DecoratedMessageToString(this string decoratedMessage)
    {
        var index = decoratedMessage.IndexOf(":", StringComparison.Ordinal) + 1;
        var subStr = decoratedMessage.Substring(index, decoratedMessage.Length - index);

        var exChunks = subStr.Split(new[] { ":" }, 3, StringSplitOptions.RemoveEmptyEntries);
        var lineVals = exChunks[1].Split(new[] { ',' }, 2);
        var exception = exChunks[2];

        return $"Details: {exception}\n\tLine: {lineVals[0].Remove(0, 1)}\n\tOffset: {lineVals[1].Remove(lineVals[1].Length - 1)}";
    }
}