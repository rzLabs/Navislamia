using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.Packets
{
    public static class Header
    {
        public static byte[] Generate(ushort id, int length, byte checksum)
        {
            byte[] buffer = new byte[2 + length + 1];

            Buffer.BlockCopy(BitConverter.GetBytes(id), 0, buffer, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(length), 0, buffer, 2, 4);
            Buffer.BlockCopy(new byte[] { checksum }, 0, buffer, 6, 1);

            return buffer;
        }
    }
}
