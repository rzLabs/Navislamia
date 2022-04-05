using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.Packets
{
    public static class Checksum
    {
        public static byte Calculate(uint size, uint id)
        {
            byte value = 0;

            value += (byte)(size & 0xFF);
            value += (byte)((size >> 8) & 0xFF);
            value += (byte)((size >> 16) & 0xFF);
            value += (byte)((size >> 24) & 0xFF);

            value += (byte)(id & 0xFF);
            value += (byte)((id >> 8) & 0xFF);

            return value;
        }
    }
}
