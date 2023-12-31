using System.Runtime.InteropServices;

namespace Navislamia.Game.Network.Packets.Game;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TS_SC_RESULT
{
    public ushort RequestMsgID;
    public ushort Result;
    public int Value;

    public TS_SC_RESULT(ushort id, ushort result, int value = 0)
    {
        RequestMsgID = id;
        Result = result;
        Value = value;
    }
}
