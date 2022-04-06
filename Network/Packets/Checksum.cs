using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.Packets
{
    public static class Checksum
    {
        /// <summary>
        /// Calculate the checksum of the packet
        /// Ported from Glandu2 CLI Packet Serializer
        /// </summary>
        /// <param name="size"></param>
        /// <param name="id"></param>
        /// <seealso cref="https://github.com/glandu2/rzu_packet_dotnet/blob/4e179816ae03de067d299342a90250e284c15ac3/lib/Packet/CliSerializer.h#L21"/>
        /// <returns></returns>
        public static byte Calculate(Packet packet)
        {
            byte value = 0;

            uint size = packet.Length;
            uint id = packet.ID;

            value += (byte)(size & 0xFF);
            value += (byte)((size >> 8) & 0xFF);
            value += (byte)((size >> 16) & 0xFF);
            value += (byte)((size >> 24) & 0xFF);

            value += (byte)(id & 0xFF);
            value += (byte)((id >> 8) & 0xFF);

            return packet.Checksum = value;
        }
    }
}
