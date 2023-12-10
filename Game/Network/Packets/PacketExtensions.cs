using Navislamia.Network.Packets.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        public static string DumpToString<T>(this IPacket packet)
        {
            var dataStruct = packet.GetDataStruct<T>();

            string output = $"[orchid]Packet Info:[/]\n\nName: {packet.GetType().Name}\n\nHeader:\n\n- [steelblue3]ID:[/] {packet.ID}\n- [steelblue3]Length:[/] {packet.Length}\n- [steelblue3]Checksum:[/] {packet.Checksum}\n\nProperties:\n\n";

            foreach (var property in dataStruct.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
            {
                string str = $"- [steelblue3]{property.Name}[/] [deeppink4_2]({property.PropertyType.Name})[/]: ";
                dynamic value = property.GetValue(packet);

                if (value is byte[])
                    continue;

                //if (value is CString)
                //    str += ((CString)value).String;
                //else
                //    str += value;

                output += $"{str}\n";
            }

            output += $"\nHex Dump:\n\n{packet.ToHexString()}\n";

            return output;
        }

        public static string ToHexString(this IPacket packet)
        {
            byte[] buffer = packet.Data;

            int maxWidth = Math.Min(16, buffer.Length);
            int rowHeader = 0;

            string outStr = null;
            string curRowStr = null;
            int curCol = 0;

            for (int i = 0; i < buffer.Length; i++)
            {
                string byteStr = $"{buffer[i]:x2}";
                curRowStr += $"{byteStr} ";
                curCol++;

                if (curCol == maxWidth)
                {
                    rowHeader += 10;

                    byte[] lineBuffer = ((Span<byte>)buffer).Slice(i + 1 - maxWidth, maxWidth).ToArray();
                    string lineBufferStr = string.Empty;

                    foreach (byte b in lineBuffer)
                    {
                        if (b == 0)
                            lineBufferStr += ".";
                        else
                            lineBufferStr += System.Text.Encoding.Default.GetString(new byte[] { b });
                    }

                    if (maxWidth < 16) // There was not 16 bytes of data, so we must pad the rest to keep output aligned
                    {
                        int remainder = 16 - maxWidth;

                        for (int j = 0; j < remainder; j++)
                        {
                            curRowStr += "00 ";
                            lineBufferStr += ".";
                        }
                    }

                    outStr += $"{rowHeader.ToString("D8")}: {curRowStr}  {lineBufferStr}\n";
                    curRowStr = null;
                    curCol = 0;

                    if (buffer.Length - i < maxWidth)
                        maxWidth = buffer.Length - (i + 1);
                }
            }

            return outStr;
        }
    }
}
