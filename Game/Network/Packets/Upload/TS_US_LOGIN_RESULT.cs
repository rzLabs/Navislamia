using System.Runtime.InteropServices;

namespace Navislamia.Game.Network.Packets.Upload;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TS_US_LOGIN_RESULT 
{
    public ushort Result;
}
