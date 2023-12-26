using Navislamia.Game.Network.Entities;
using Navislamia.Game.Network.Packets;

namespace Navislamia.Game.Network.Interfaces;

public interface IUploadActionService
{
    void Execute(UploadClient client, IPacket packet);
    void OnLoginResult(UploadClient client, IPacket packet);
}