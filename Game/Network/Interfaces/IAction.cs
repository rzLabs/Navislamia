using Navislamia.Game.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Navislamia.Game.Network.Clients;
using Navislamia.Game.Network.Packets.Interfaces;

namespace Navislamia.Game.Network.Interfaces
{
    public interface IActions
    {
        void Execute(Client client, IPacket packet);
    }
}
