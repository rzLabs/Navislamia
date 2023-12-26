using Navislamia.Game.Network.Entities;
using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Packets.Actions;

namespace Navislamia.Game.Network.Interfaces;

public interface IGameActionService
{
    GameActionState State { get; set; }
    void Execute(GameClient client, IPacket packet);
}