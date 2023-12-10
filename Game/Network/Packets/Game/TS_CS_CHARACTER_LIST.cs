using Navislamia.Network.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets.Game
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TS_CS_CHARACTER_LIST
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
        public string Account;
    }
}
