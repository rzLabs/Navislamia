using System.Runtime.InteropServices;


namespace Navislamia.Game.Network.Packets.Game
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TS_LOGIN
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 19)]
        public string Name;

        public byte Race;
    }
}
