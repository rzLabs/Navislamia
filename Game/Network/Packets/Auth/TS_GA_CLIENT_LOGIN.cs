using System.Runtime.InteropServices;

namespace Navislamia.Game.Network.Packets;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TS_GA_CLIENT_LOGIN
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
    public string Account;

    public long OneTimeKey;

    public TS_GA_CLIENT_LOGIN(string account, long oneTimeKey)
    {
        Account = account;
        OneTimeKey = oneTimeKey;

    }
}
