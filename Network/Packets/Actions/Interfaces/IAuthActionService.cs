using Navislamia.Network.Entities;
using Navislamia.Network.Interfaces;

namespace Navislamia.Network.Packets.Actions.Interfaces
{
    public interface IAuthActionService
    {
        public int Execute(ClientService<AuthClientEntity> entity, ISerializablePacket msg);
    }
}
