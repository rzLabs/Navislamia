using Navislamia.Game.Network.Entities;
using Navislamia.Game.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.Network.Interfaces
{
    public interface IActions
    {
        void Execute(Client client, IPacket packet);
    }
}
