using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Navislamia.Game.Network.Entities;

namespace Navislamia.Game.Network.Interfaces;

public interface IGameClientService
{
    ClientEntity CreateGameClient(Socket socket);
    void RemoveGameClient(ClientEntity client);
    List<ClientEntity> GetAuthorizedClients();
    IEnumerable<ClientEntity> GetUnauthorizedClients();
    void AuthorizeClient(string accountName, ClientEntity client);
    void UnAuthorizeClient(string accountName, ClientEntity client);
    bool IsAuthorized(string accountName);
    bool IsUnauthorized(string accountName);
    void SendResult(ClientEntity client, ushort id, ushort result, int value = 0);
}