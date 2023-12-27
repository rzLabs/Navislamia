using System.Collections.Generic;
using System.Net.Sockets;
using Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Navislamia.Game.Network.Entities;
using Navislamia.Game.Network.Entities.Actions;
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
    private readonly NetworkOptions _networkOptions;

    private AuthClient AuthClient { get; set; }
    private UploadClient UploadClient { get; set; }
    private List<GameClient> GameClients { get; set; } = new();
    
    private readonly AuthActions _authActions;
    private readonly UploadActions _uploadActions;
    private readonly GameActions _gameActions;


    public NetworkService(ILogger<NetworkService> logger, IOptions<NetworkOptions> networkOptions, ICharacterService characterService)
    {
        _logger = logger;
        _characterService = characterService;
        _networkOptions = networkOptions.Value;

        _authActions = new AuthActions(GameClients);
        _uploadActions = new UploadActions();
        _gameActions = new GameActions(_characterService, GameClients);
    }

    public bool IsReady()
    {
        return AuthClient.Ready && UploadClient.Ready;
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

        AuthClient = new AuthClient(_networkOptions.Auth.Ip, _networkOptions.Auth.Port, GameClients, _authActions);
        _gameActions.AuthClient = AuthClient;
    }

    public void CreateUploadClient()
    {
        if (UploadClient != null)
        {
            _logger.LogWarning("Client already exists. Skipping creation");
            return;
        }

        UploadClient = new UploadClient(_networkOptions.Upload.Ip, _networkOptions.Upload.Port, _uploadActions);
    }

    public GameClient CreateGameClient(Socket socket)
    {
        var client = new GameClient(socket, _networkOptions.CipherKey, _networkOptions.MaxConnections,
            _characterService, AuthClient, GameClients, _gameActions);
        
        client.CreateClientConnection();
        
        AuthClient.SetAffectedGameClient(client);

        return client;
    }

}