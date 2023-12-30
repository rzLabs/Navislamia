using System.Runtime.InteropServices;

namespace Navislamia.Game.Network.Packets.Game;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TM_CS_ACCOUNT_WITH_AUTH
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
    public string Account;

    public long OneTimePassword;

}
