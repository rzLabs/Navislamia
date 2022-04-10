using Navislamia.Network.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets
{
    public static class PacketExtension
    {
        public static string DumpToString(this Packet packet)
        {
            string output = $"Printing packet: {packet.GetType().Name} ({packet.ID})\n\nChecksum: 0x{packet.Checksum.ToString("X2")}\n\n";

            foreach (var property in packet.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
            {
                string str = $"{property.Name} ({property.PropertyType.Name}):\n\n";
                dynamic value = property.GetValue(packet);

                if (value is byte[])
                    continue;

                if (value is CString)
                {
                    CString cVal = value as CString;

                    str += $"Simple: {cVal.String}\n\nDetailed:\n\n{Utilities.StringExt.ByteArrayToString(cVal.Data)}";
                }
                else
                {
                    byte[] bVal = BitConverter.GetBytes(value);

                    str += $"Simple: {value}\n\nDetailed:\n\n{Utilities.StringExt.ByteArrayToString(bVal)}";
                }

                output += $"{str}\n";
            }

            return output;
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
    }
}
