using System.Runtime.InteropServices;

namespace Navislamia.Game.Network.Packets.Auth;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TS_AG_CLIENT_LOGIN
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
    public string Account;

    public int AccountID;

    public ushort Result;

    public byte PcBangMode;

    public int EventCode;

    public int Age;

    public float ContinuousPlayTime;

    public float ContinuousLogoutTime;
}
