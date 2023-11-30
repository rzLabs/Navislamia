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
    public class PacketHeader
    {
        public uint Length;
        public ushort ID;
        public byte Checksum;
    }

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

        public static PacketHeader GetPacketHeader(Span<byte> buffer)
        {
            if (buffer.Length < 7)
                throw new Exception("Not enough data to form header!");

            return new PacketHeader() { Length = BitConverter.ToUInt32(buffer.Slice(0, 4)),
                                      ID = BitConverter.ToUInt16(buffer.Slice(4,2)),
                                      Checksum = buffer.Slice(6,1)[0] 
            };
        }

        public static void GetHeaderInfo(Span<byte> span,out int length, out ushort id, out byte checksum)
        {
            length = BitConverter.ToInt32(span.Slice(0, 4));
            id = BitConverter.ToUInt16(span.Slice(4, 2));
            checksum = span.Slice(6, 1)[0];
        }
    }
}
