using System.Collections.Generic;
using Navislamia.Game.Network.Entities;
using Navislamia.Game.Network.Enums;
using Navislamia.Network.Packets;
using Navislamia.Network.Packets.Actions;

namespace Navislamia.Game.Network
{
    public interface INetworkModule
    {
        bool Start();
    }
}
