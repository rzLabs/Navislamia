using System.Runtime.InteropServices;

namespace Navislamia.Game.Network.Packets;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TS_SU_LOGIN
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
    public string ServerName;

    public TS_SU_LOGIN(string serverName)
    {
        ServerName = serverName;
    }
}
