using System;

namespace Utilities
{
    /// <summary>
    /// Set of methods to provide string manipulation
    /// </summary>
    public static class StringExt
    {
        static string[] sizes = { "B", "KB", "MB", "GB", "TB" };

        /// <summary>
        /// Convert a files raw length to a formatted string like 1MB
        /// </summary>
        /// <param name="length">Length to be converted</param>
        /// <returns>Formatted size string</returns>
        public static string SizeToString(long length)
        {
            double len = length;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
                           
            return String.Format("{0:0.##} {1}", len, sizes[order]);
        }

        /// <summary>
        /// Convert a timespan in miliseconds to a formatted string like 1s 200ms
        /// </summary>
        /// <param name="miliseconds">Miliseconds count to be converted</param>
        /// <returns>Formatted time string</returns>
        public static string MilisecondsToString(long miliseconds)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(miliseconds);

            string timeStr = null;

            if (miliseconds > 1000)
                timeStr = string.Format("{0:D3} seconds {1:D4}ms", t.Seconds, t.Milliseconds);
            else if (miliseconds > 60000)
                timeStr = string.Format("{0:D3} minutes {1:D3} seconds {2:D4}ms", t.Minutes, t.Seconds, t.Milliseconds);
            else
                timeStr = string.Format("{0:D4}ms", t.Milliseconds);

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
            int index = decoatedMessage.IndexOf(":") + 1;
            string subStr = decoatedMessage.Substring(index, decoatedMessage.Length - index);

            string[] exChunks = subStr.Split(new string[] { ":" }, 3, StringSplitOptions.RemoveEmptyEntries);
            string[] lineVals = exChunks[1].Split(new char[] { ',' }, 2);
            string exception = exChunks[2];

            return $"Details: {exception}\n\tLine: {lineVals[0].Remove(0, 1)}\n\tOffset: {lineVals[1].Remove(lineVals[1].Length - 1)}";
        }

        /// <summary>
        /// Convert a byte array to a prepared string capable of representing the array via hex values.
        /// </summary>
        /// <param name="buffer">Byte array containing information</param>
        /// <returns>Formatted string representing the provided byte array</returns>
        public static string ByteArrayToString(byte[] buffer, int count = 512)
        {
            string outStr = null;
            string curRowStr = null;
            int curCol = 0;

            for (int i = 0; i < buffer.Length; i++)
            {
                string byteStr = $"0x{buffer[i].ToString("x2")}";
                curRowStr += $"{byteStr},";
                curCol++;

                if (curCol == 16)
                {
                    outStr += $"{curRowStr}\n";
                    curRowStr = null;
                    curCol = 0;
                }

                if (i == count)
                    break;
            }

            outStr = outStr.Remove(outStr.Length - 2, 1);

            return outStr;
        }

        public static string ByteArrayToString(byte[] buffer)
        {
            int maxWidth = Math.Min(16, buffer.Length);
            int rowHeader = 0;

            string outStr = null;
            string curRowStr = null;
            int curCol = 0;

            for (int i = 0; i < buffer.Length; i++)
            {
                string byteStr = $"0x{buffer[i]:x2}";
                curRowStr += $"{byteStr},";
                curCol++;

                if (curCol == maxWidth)
                {
                    rowHeader += 10;

                    curRowStr = curRowStr.Remove(curRowStr.Length - 1, 1);

                    outStr += $"{rowHeader.ToString("D8")}: {curRowStr}\n";
                    curRowStr = null;
                    curCol = 0;
                }
            }

            return outStr;
        }

        public static string GetStringContent(string line, string header)
        {
            if (line.StartsWith(header))
                return line.Substring(header.Length);

            return null;
        }
    }
}
