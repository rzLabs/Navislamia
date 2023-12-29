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
            var _length = name.Length;

            if (_length < minLength || _length > maxLength)
            {
                return false;
            }

            if (name.Any(c => !char.IsLetterOrDigit(c)))
            {
                return false;
            }

            return true;
        }

        public static string FormatName(this string name)
        {
            return $"{char.ToUpper(name[0])}{name.Substring(1)}";
        }
    }
}