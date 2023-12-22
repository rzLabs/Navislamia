using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Navislamia.Network.Packets;
using Navislamia.Network.Packets.Actions;

namespace Navislamia.Game.Network.Entities;

public interface IClientService
{
    int ClientCount { get; }

    AuthClientService AuthClient { get; }

    UploadClientService UploadClient { get; }

    bool AuthReady { get; set; }

    bool UploadReady { get; set; }

    bool IsReady { get; }

    Dictionary<string, GameClientService> UnauthorizedGameClients { get; set; }

    Dictionary<string, GameClientService> AuthorizedGameClients { get; set; }

    AuthClientService CreateAuthClient(IPEndPoint authEndPoint);

    UploadClientService CreateUploadClient(IPEndPoint authEndPoint);

    GameClientService CreateGameClient(Socket scoket);

    bool RegisterGameClient(string account,  GameClientService client);

    void RemoveGameClient(string account, GameClientService client);
}