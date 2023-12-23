using System.Runtime.InteropServices;

namespace Navislamia.Game.Network.Packets;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TS_AG_KICK_CLIENT
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
    public string Account;

    public KickType KickType;
}

public enum KickType
{
    AnotherLogin = 0,
    DuplicatedLogin = 1,
    BillingExpired = 2,
    GameAddiction = 3,
};