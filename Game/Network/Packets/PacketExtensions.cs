using System;
using System.Runtime.InteropServices;

namespace Navislamia.Game.Network.Packets;

public static class PacketExtensions
{
    /// <summary>
    /// Calculate the checksum of the packet
    /// Ported from Glandu2 CLI Packet Serializer
    /// </summary>
    /// <seealso cref="https://github.com/glandu2/rzu_packet_dotnet/blob/4e179816ae03de067d299342a90250e284c15ac3/lib/Packet/CliSerializer.h#L21"/>
    /// <returns></returns>
    public static byte CalculateChecksum(this ref Header header)
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
