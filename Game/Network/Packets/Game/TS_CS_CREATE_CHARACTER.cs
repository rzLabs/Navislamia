using System.Runtime.InteropServices;

using Navislamia.Game.Network.Entities;

namespace Navislamia.Game.Network.Packets.Game
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TS_CS_CREATE_CHARACTER
    {
        public LobbyCharacterInfo Info;
    }
}
