using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets;

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
