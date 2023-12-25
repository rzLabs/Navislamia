using Navislamia.Game.Network.Entities;
using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Packets.Actions;

namespace Navislamia.Game.Network.Interfaces;

public interface IGameActionService
{
    GameActionState State { get; set; }
    void Execute(ClientEntity client, IPacket packet);
    void SendResult(ClientEntity client, ushort id, ushort result, int value = 0);
}