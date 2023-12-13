using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Notification
{
    internal static class StringUtilities
    {
        public static string RemoveFormatting(this string formattedString)
        {
            int startIdx = -1;
            int endIdx = -1;

            string outString = formattedString;

            while (true)
            {
                startIdx = outString.IndexOf('[');
                endIdx = outString.IndexOf(']') + 1;

                if (endIdx == 0)
                    endIdx = startIdx + 1;

                if (startIdx >= 0 && endIdx >= 0)
                    outString = outString.Remove(startIdx, (endIdx - startIdx));
                else
                    break;
            }

            return outString;
        }
    }
}
