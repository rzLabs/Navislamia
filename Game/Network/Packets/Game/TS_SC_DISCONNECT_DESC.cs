using Navislamia.Game.Network.Packets.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.Network.Packets.Game
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TS_SC_DISCONNECT_DESC
    {
        public byte DescId;

        public TS_SC_DISCONNECT_DESC(DisconnectType type)
        {
            DescId = (byte)type;
        }
    }
}
