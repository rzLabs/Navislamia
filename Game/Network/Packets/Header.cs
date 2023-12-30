using System;
using System.Runtime.InteropServices;

namespace Navislamia.Game.Network.Packets;

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
}
