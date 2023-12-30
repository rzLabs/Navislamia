using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Navislamia.Game.Network.Clients;
using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Packets.Interfaces;

namespace Navislamia.Game.Network.Interfaces;

public interface INetworkService
{
    // int ClientCount { get; }
    bool IsReady();
    
    void CreateAuthClient();
    void CreateUploadClient();
    GameClient CreateGameClient(Socket socket);

    void SendMessageToAuth(IPacket packet);

    void SendMessageToUpload(IPacket packet);
    
}