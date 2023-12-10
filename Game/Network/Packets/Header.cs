using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using Navislamia.Network.Packets.Auth;

namespace Navislamia.Network.Packets
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Header
    {
        public uint Length;
        public ushort ID;
        public byte Checksum;

        public Header(ushort id)
        {
            Length = 0;
            ID = id;
            Checksum = 0;
        }
    }
}
