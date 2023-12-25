using Navislamia.Game.Network.Packets;

namespace Navislamia.Game.Network.Interfaces;

public interface IAuthActionService
{
    void Execute(AuthClientService clientService, IPacket packet);
}