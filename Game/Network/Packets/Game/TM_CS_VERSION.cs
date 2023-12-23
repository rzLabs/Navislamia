using System.Runtime.InteropServices;

namespace Navislamia.Network.Packets;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TM_CS_VERSION
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    public string Version;

}
