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

            output += $"\nHex Dump:\n\n{Utilities.StringExt.ByteArrayToString(packet.Data)}\n";

            return output;
        }

        public static string ToHexString()
        {
            throw new NotImplementedException();

			//byte[] char2hex = new []{
   //             0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17,
   //             18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35,
   //             36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53,
   //             54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71,
   //             72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89,
   //             90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107,
   //             108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125,
   //             126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143,
   //             144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161,
   //             162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179,
   //             180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197,
   //             198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215,
   //             216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233,
   //             234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251,
   //             252, 253, 254, 255,
   //         };

			//if (packetLogger == nullptr || !packetLogger->wouldLog(level))
			//	return;

			//char messageBuffer[4096];

			//va_list args;

			//va_start(args, format);
			//vsnprintf(messageBuffer, 4096, format, args);
			//messageBuffer[4095] = 0;
			//va_end(args);

			//if (rawData && size > 0)
			//{
			//	std::string buffer;
			//	buffer.reserve((size + 15) / 16 * 74 + 1);

			//	// Log full packet data
			//	const int lineNum = (size + 15) / 16;

			//	for (int line = 0; line < lineNum; line++)
			//	{
			//		int maxCharNum = size - (line * 16);
			//		if (maxCharNum > 16)
			//			maxCharNum = 16;

			//		buffer += char2hex[(line * 16 >> 8) & 0xFF];
			//		buffer += char2hex[line * 16 & 0xFF];
			//		buffer += ": ";

			//		for (int row = 0; row < 16; row++)
			//		{
			//			if (row < maxCharNum)
			//			{
			//				buffer += char2hex[rawData[line * 16 + row]];
			//				buffer += " ";
			//			}
			//			else
			//				buffer += "   ";
			//			if (row == 7)
			//				buffer += ' ';
			//		}

			//		buffer += ' ';

			//		for (int row = 0; row < maxCharNum; row++)
			//		{
			//			const unsigned char c = rawData[line * 16 + row];

			//			if (c >= 32 && c < 127)
			//				buffer += c;
			//			else
			//				buffer += '.';
			//			if (row == 7)
			//				buffer += ' ';
			//		}
			//		buffer += '\n';
			//	}
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

        public static bool ChecksumPassed(this ISerializablePacket msg, PacketHeader header) => msg.Checksum == header.Checksum;
   
        public static bool IsValid(this ISerializablePacket msg)
        {
            return msg.Checksum == Checksum.Calculate(msg as Packet);
        }
    }
}
