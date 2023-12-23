using System.Runtime.InteropServices;

namespace Navislamia.Game.Network.Packets;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TS_GA_CLIENT_KICK_FAILED
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
    public string Account;
}