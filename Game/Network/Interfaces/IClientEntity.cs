using Navislamia.Game.Network.Interfaces;
using Navislamia.Game.Network.Packets;

namespace Navislamia.Game.Network.Entities;

public interface IClientEntity
{
    IConnection Connection { get; }

    string ClientTag { get; }

    void SendMessage(IPacket msg);
}
