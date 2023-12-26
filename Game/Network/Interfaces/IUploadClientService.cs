using Navislamia.Game.Network.Entities;

namespace Navislamia.Game.Network.Interfaces;

public interface IUploadClientService
{
    void CreateUploadClient();
    UploadClient GetClient();
}