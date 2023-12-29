using System.Runtime.InteropServices;

namespace Navislamia.Game.Network.Packets.Auth;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TS_AG_LOGIN_RESULT
{
    public ushort Result;
}
