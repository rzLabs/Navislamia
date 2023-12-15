using Navislamia.Network.Packets.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Navislamia.Network.Packets
{
    public static class PacketExtensions
    {
        public static Header PeekHeader(this byte[] data)
        {
            if (data.Length < 7)
                throw new Exception("Not enough data to peek header!");

            var _header = new Header();
            _header.Length = BitConverter.ToUInt32(data, 0);
            _header.ID = BitConverter.ToUInt16(data, 4);
            _header.Checksum = data[6];

            return _header;
        }

        /// <summary>
        /// Calculate the checksum of the packet
        /// Ported from Glandu2 CLI Packet Serializer
        /// </summary>
        /// <seealso cref="https://github.com/glandu2/rzu_packet_dotnet/blob/4e179816ae03de067d299342a90250e284c15ac3/lib/Packet/CliSerializer.h#L21"/>
        /// <returns></returns>
        public static void CalculateChecksum(this ref Header header)
        {
            header.Checksum += (byte)(header.Length & 0xFF);
            header.Checksum += (byte)((header.Length >> 8) & 0xFF);
            header.Checksum += (byte)((header.Length >> 16) & 0xFF);
            header.Checksum += (byte)((header.Length >> 24) & 0xFF);
            header.Checksum += (byte)(header.ID & 0xFF);
            header.Checksum += (byte)((header.ID >> 8) & 0xFF);
        }

        public static byte[] StructToByte<T>(this T structure)
        {
            var length = Marshal.SizeOf(structure);
            var ptr = Marshal.AllocHGlobal(length);

            byte[] buffer = new byte[length];

            Marshal.StructureToPtr(structure, ptr, true);
            Marshal.Copy(ptr, buffer, 0, length);
            Marshal.FreeHGlobal(ptr);

            return buffer;
        }
    }
}
