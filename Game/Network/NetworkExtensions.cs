using Navislamia.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network
{
    public static class NetworkExtensions
    {
        public static string EnumToString(this Enum input) => Enum.GetName(typeof(ClientType), input);
    }
}
