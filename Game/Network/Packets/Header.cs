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

        public Header(ReadOnlySpan<byte> data)
        {
            Length = BitConverter.ToUInt32(data.Slice(0, 4));
            ID = BitConverter.ToUInt16(data.Slice(4, 2));
            Checksum = data[6];
        }

        /// <summary>
        /// Calculate the checksum of the packet
        /// Ported from Glandu2 CLI Packet Serializer
        /// </summary>
        /// <seealso cref="https://github.com/glandu2/rzu_packet_dotnet/blob/4e179816ae03de067d299342a90250e284c15ac3/lib/Packet/CliSerializer.h#L21"/>
        /// <returns></returns>
        public static byte CalculateChecksum(Header header)
        {
            byte _checksum = 0;

            _checksum += (byte)(header.Length & 0xFF);
            _checksum += (byte)((header.Length >> 8) & 0xFF);
            _checksum += (byte)((header.Length >> 16) & 0xFF);
            _checksum += (byte)((header.Length >> 24) & 0xFF);
            _checksum += (byte)(header.ID & 0xFF);
            _checksum += (byte)((header.ID >> 8) & 0xFF);

            return _checksum;
        }
    }
}
