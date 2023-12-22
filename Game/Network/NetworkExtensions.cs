using Navislamia.Game.Models.Arcadia;
using Navislamia.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network
{
    public static class NetworkExtensions
    {
        public static string EnumToString(this Enum input) => Enum.GetName(typeof(ClientType), input);

        public static bool IsConnected(this Socket socket, SelectMode selectMode = SelectMode.SelectRead)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch
            {
                return false;
            }
        }
    }
}
