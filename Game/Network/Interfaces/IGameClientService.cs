using System.Net.Sockets;
using Navislamia.Game.Network.Entities;

namespace Navislamia.Game.Network.Interfaces;

public interface IGameClientService
{
    GameClient CreateGameClient(Socket socket);
}