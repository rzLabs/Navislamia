using System.Runtime.InteropServices;

namespace Navislamia.Game.Network.Packets.Game;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TS_CS_VERSION
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    public string Version;

}
