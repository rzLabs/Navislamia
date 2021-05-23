using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Data
{
    public static class StringUtil
    {
        public static string GetStringContent(string line, string header)
        {
            if (line.StartsWith(header))
                return line.Substring(header.Length);

            return null;
        }
    }
}
