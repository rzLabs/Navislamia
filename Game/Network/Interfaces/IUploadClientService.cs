using Navislamia.Game.Network.Entities;

namespace Navislamia.Game.Network.Interfaces;

public interface IUploadClientService: IBaseClientService
{
    void CreateUploadClient();
    ClientEntity GetClient();
}