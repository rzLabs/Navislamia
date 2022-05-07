using Navislamia.Network.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets.Actions.Interfaces
{
    public interface IGameActionService
    {
        public int Execute(GameClient client, ISerializablePacket msg);
    }
}
