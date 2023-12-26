using Navislamia.Game.Network.Entities;

namespace Navislamia.Game.Network.Interfaces;

public interface IAuthClientService
{
    void CreateAuthClient();
    AuthClient GetClient();
}