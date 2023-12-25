using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Navislamia.Game.Network.Entities;
using Navislamia.Game.Network.Packets;

namespace Navislamia.Game.Network.Interfaces;

public interface INetworkService
{
    // int ClientCount { get; }
    bool IsReady();
    
    void CreateAuthClient();
    void CreateUploadClient();
    ClientEntity CreateGameClient(Socket socket);

    ClientEntity GetAuthClient();
    ClientEntity GetUploadClient();

    void SendMessageToAuth(IPacket packet);

    void SendMessageToUpload(IPacket packet);
    
}