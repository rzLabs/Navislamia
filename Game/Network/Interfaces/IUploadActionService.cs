using Navislamia.Game.Network.Packets;

namespace Navislamia.Game.Network.Interfaces;

public interface IUploadActionService
{
    void Execute(UploadClientService clientService, IPacket packet);
    void OnLoginResult(UploadClientService clientService, IPacket packet);
}