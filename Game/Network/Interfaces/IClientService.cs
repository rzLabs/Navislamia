using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Navislamia.Game.Network.Interfaces;

public interface IClientService
{
    int ClientCount { get; }

    AuthClient AuthClient { get; }

    UploadClient UploadClient { get; }

    bool AuthReady { get; set; }

    bool UploadReady { get; set; }

    bool IsReady { get; }

    Dictionary<string, GameClient> UnauthorizedGameClients { get; set; }

    Dictionary<string, GameClient> AuthorizedGameClients { get; set; }

    AuthClient CreateAuthClient(IPEndPoint authEndPoint);

    UploadClient CreateUploadClient(IPEndPoint authEndPoint);

    GameClient CreateGameClient(Socket scoket);

    bool RegisterGameClient(string account,  GameClient client);

    void RemoveGameClient(string account, GameClient client);
}