using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.Network.Packets.Game
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TS_CS_DELETE_CHARACTER
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 19)]
        public string Name;
    }
}
