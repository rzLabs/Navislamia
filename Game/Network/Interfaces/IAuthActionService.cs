using Navislamia.Game.Network.Entities;
using Navislamia.Game.Network.Packets;

namespace Navislamia.Game.Network.Interfaces;

public interface IAuthActionService
{
    void Execute(AuthClient client, IPacket packet);
}