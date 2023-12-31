using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.Network.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidName(this string name, int minLength, int maxLength)
        {
            return name.Length >= minLength && name.Length <= maxLength && name.All(char.IsLetterOrDigit);
        }

        public static string FormatName(this string name)
        {
            return $"{char.ToUpper(name[0])}{name.Substring(1)}";
        }
    }
}