using Navislamia.Game.Network.Interfaces;

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
