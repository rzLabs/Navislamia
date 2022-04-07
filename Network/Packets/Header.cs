using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace Navislamia.Network.Packets
{
    public static class Header
    {
        public static byte[] CreateHeader(this Packet packet)
        {
            byte[] buffer = new byte[7];

            Buffer.BlockCopy(BitConverter.GetBytes(packet.Length), 0, buffer, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(packet.ID), 0, buffer, 4, 2);
            Buffer.BlockCopy(new byte[] { Checksum.Calculate(packet) }, 0, buffer, 6, 1);

            return buffer;
        }
    }
}
