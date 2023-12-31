using System.Runtime.InteropServices;

namespace Navislamia.Game.Network.Packets.Auth;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TS_GA_CLIENT_LOGOUT
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
    public string Account;
    public uint ContinousPlayTime;

    public TS_GA_CLIENT_LOGOUT(string account, uint continousPlayTime)
    {
        Account = account;
        ContinousPlayTime = continousPlayTime;
    }
}
