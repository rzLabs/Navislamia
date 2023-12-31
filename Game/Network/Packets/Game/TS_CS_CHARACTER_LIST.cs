using System.Runtime.InteropServices;

namespace Navislamia.Game.Network.Packets.Game;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TS_CS_CHARACTER_LIST
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
    public string Account;
}
