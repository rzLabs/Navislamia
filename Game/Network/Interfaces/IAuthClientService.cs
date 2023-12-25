using Navislamia.Game.Network.Entities;

namespace Navislamia.Game.Network.Interfaces;

public interface IAuthClientService : IBaseClientService
{
    void CreateAuthClient();
    ClientEntity GetClient();
}