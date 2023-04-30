using Navislamia.Network.Entities;
using Navislamia.Network.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets.Actions.Interfaces
{
    public interface IGameActionService
    {
        public int Execute(ClientService<GameClientEntity> client, ISerializablePacket msg);
    }
}
