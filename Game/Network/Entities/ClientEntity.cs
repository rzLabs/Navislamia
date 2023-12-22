using System;
using System.Net;
using System.Net.Sockets;
using Navislamia.Network.Enums;
using Navislamia.Network.Packets;

namespace Navislamia.Game.Network.Entities
{
    public class ClientEntity
    {
        public uint PacketVersion;

        public IConnection Connection { get; private set; }

        public ClientEntity(IConnection connection, uint packetVersion = 0)
        {
            Connection = connection;
        }
    }
    
}
