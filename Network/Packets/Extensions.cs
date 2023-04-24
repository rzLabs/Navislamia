using Navislamia.Network.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets
{
    public static class Extensions
    {
        

        public static string DumpToString(this Packet packet)
        {
            string output = $"[orchid]Packet Info:[/]\n\nName: {packet.GetType().Name}\n\nHeader:\n\n- [steelblue3]ID:[/] {packet.ID}\n- [steelblue3]Length:[/] {packet.Length}\n- [steelblue3]Checksum:[/] {packet.Checksum}\n\nProperties:\n\n";

            foreach (var property in packet.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
            {
                string str = $"- [steelblue3]{property.Name}[/] [deeppink4_2]({property.PropertyType.Name})[/]: ";
                dynamic value = property.GetValue(packet);

                if (value is byte[])
                    continue;

                if (value is CString)
					str += ((CString)value).String;
				else
                    str += value;

                output += $"{str}\n";
            }

            output += $"\nHex Dump:\n\n{packet.ToHexString()}\n";

            return output;
        }

        public static string ToHexString(this Packet packet)
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

        public static uint GetLength(this Packet packet)
        {
            uint length = 7;

            foreach (var property in packet.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
            {
                var value = property.GetValue(packet);

                if (value is byte[])
                    continue;

                if (value is CString)
                    length += Convert.ToUInt32(((CString)value).Length);
                else if (value is bool)
                    length += 1;
                else
                    length += Convert.ToUInt32(Marshal.SizeOf(value));
            }

            return length;
        }
   
        public static bool IsValid(this ISerializablePacket msg)
        {
            return msg.Checksum == Checksum.Calculate(msg as Packet);
        }
    }
}
