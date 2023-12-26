using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Navislamia.Game.Network.Entities;
using Navislamia.Game.Network.Interfaces;
using Navislamia.Game.Network.Packets;
using Navislamia.Game.Services;

namespace Navislamia.Game.Network;

// TODO: Poll connections by configuration interval
// TODO: Disconnect/Destroy

public class NetworkService : INetworkService
{
    private readonly ILogger<NetworkService> _logger;
    private readonly ICharacterService _characterService;
    private AuthClient AuthClient { get; set; }
    private UploadClient UploadClient { get; set; }
    private List<GameClient> GameClients { get; set; } = new();
    
    private readonly NetworkOptions _networkOptions;

    public NetworkService(ILogger<NetworkService> logger, IOptions<NetworkOptions> networkOptions, ICharacterService characterService)
    {
        _logger = logger;
        _characterService = characterService;
        _networkOptions = networkOptions.Value;
    }
    
    public bool IsReady()
    {
        return AuthClient.Connection.Connected && UploadClient.Connection.Connected;
    }
    
    public void SendMessageToAuth(IPacket packet)
    {
        AuthClient.SendMessage(packet);
    }
    
    public void SendMessageToUpload(IPacket packet)
    {
        UploadClient.SendMessage(packet);
    }

    public void CreateAuthClient()
    {
        if (AuthClient != null)
        {
            _logger.LogWarning("AuthClient already exists. Skipping creation");
            return;
        }

        AuthClient = new AuthClient(GameClients);
        AuthClient.CreateClientConnection(_networkOptions.Auth.Ip, _networkOptions.Auth.Port);
    }

    public void CreateUploadClient()
    {
        if (UploadClient != null)
        {
            _logger.LogWarning("Client already exists. Skipping creation");
            return;
        }

        UploadClient = new UploadClient();
        UploadClient.CreateClientConnection(_networkOptions.Upload.Ip, _networkOptions.Upload.Port);
    }

    public GameClient CreateGameClient(Socket socket)
    {
        var client = new GameClient(socket, _networkOptions.CipherKey, _networkOptions.MaxConnections,
            _characterService, AuthClient);
        client.CreateClientConnection();
        GameClients.Add(client);
        
        AuthClient.SetAffectedGameClient(client);

        return client;
    }

}