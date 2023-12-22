using Navislamia.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets.Game
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TM_CS_VERSION
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Version;

    }
}
