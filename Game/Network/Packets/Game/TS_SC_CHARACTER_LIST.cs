using System.Runtime.InteropServices;

namespace Navislamia.Game.Network.Packets.Game;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TS_SC_CHARACTER_LIST
{
    public uint CurrentServerTime;
    public ushort LastLoginIndex;
    public ushort Count;

    public TS_SC_CHARACTER_LIST(uint currentServerTime, ushort lastLoginIndex, ushort count)
    {
        CurrentServerTime = currentServerTime;
        LastLoginIndex = lastLoginIndex;
        Count = count;
    }
}
