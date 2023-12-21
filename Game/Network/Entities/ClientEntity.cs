using System.Net;
using System.Net.Sockets;
using Navislamia.Network.Enums;

namespace Navislamia.Game.Network.Entities
{
    public class ClientEntity
    {
        public uint PacketVersion;

        public IConnection Connection;

        public ClientType Type { get; set; }

    }
    
}
