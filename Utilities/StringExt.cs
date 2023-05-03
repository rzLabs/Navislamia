using System;
using System.Text;

namespace Navislamia.Utilities
{
    /// <summary>
    /// Set of methods to provide string manipulation
    /// </summary>
    public static class StringExt
    {
        private static readonly string[] Sizes = { "B", "KB", "MB", "GB", "TB" };

        /// <summary>
        /// Convert a files raw length to a formatted string like 1MB
        /// </summary>
        /// <param name="length">Length to be converted</param>
        /// <returns>Formatted size string</returns>
        public static string SizeToString(long length)
        {
            double len = length;
            int order = 0;

            while (len >= 1024 && order < Sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
                           
            return $"{len:0.##} {Sizes[order]}";
        }

        /// <summary>
        /// Convert a timespan in miliseconds to a formatted string like 1s 200ms
        /// </summary>
        /// <param name="milliseconds">Miliseconds count to be converted</param>
        /// <returns>Formatted time string</returns>
        public static string MilisecondsToString(long milliseconds)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(milliseconds);

            string timeStr = null;

            if (milliseconds > 1000)
                timeStr = $"{t.Seconds:D3} seconds {t.Milliseconds:D4}ms";
            else if (milliseconds > 60000)
                timeStr = $"{t.Minutes:D3} minutes {t.Seconds:D3} seconds {t.Milliseconds:D4}ms";
            else
                timeStr = $"{t.Milliseconds:D4}ms";

            return timeStr;
        }

        /// <summary>
        /// Convert provided decorated string into user friendly view 
        /// </summary>
        /// <seealso cref="MoonSharp.Interpreter.InterpreterException"/>
        /// <param name="decoatedMessage">Decorated Moonsharp exception message</param>
        /// <returns>String containing relevant information to the exception</returns>
        public static string LuaExceptionToString(string decoatedMessage)
        {
            int index = decoatedMessage.IndexOf(":", StringComparison.Ordinal) + 1;
            string subStr = decoatedMessage.Substring(index, decoatedMessage.Length - index);

            string[] exChunks = subStr.Split(new string[] { ":" }, 3, StringSplitOptions.RemoveEmptyEntries);
            string[] lineVals = exChunks[1].Split(new char[] { ',' }, 2);
            string exception = exChunks[2];

            return $"Details: {exception}\n\tLine: {lineVals[0].Remove(0, 1)}\n\tOffset: {lineVals[1].Remove(lineVals[1].Length - 1)}";
        }

        public static string ByteArrayToString(this byte[] buffer)
        {
            int maxWidth = Math.Min(16, buffer.Length);
            int rowHeader = 0;

            string outStr = null;
            string curRowStr = null;
            int curCol = 0;

            for (int i = 0; i < buffer.Length; i++)
            {
                string byteStr = $"{buffer[i]:x2}";
                curRowStr += $"{byteStr} ";
                curCol++;

                if (curCol == maxWidth)
                {
                    rowHeader += 10;

                    byte[] lineBuffer = ((Span<byte>)buffer).Slice(i + 1 - maxWidth, maxWidth).ToArray();
                    string lineBufferStr = string.Empty;

                    foreach (byte b in lineBuffer)
                    {
                        if (b == 0)
                            lineBufferStr += ".";
                        else
                            lineBufferStr += Encoding.Default.GetString(new byte[] { b });
                    }

                    outStr += $"{rowHeader:D8}: {curRowStr}  {lineBufferStr}\n";
                    curRowStr = null;
                    curCol = 0;
                }
            }

            return outStr;
        }

        public static string GetStringContent(string line, string header)
        {
            return line.StartsWith(header) ? line[header.Length..] : null;
        }
    }
}
