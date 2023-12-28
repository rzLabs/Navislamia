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
    public readonly NetworkOptions Options;

    public AuthClient AuthClient { get; set; }
    public UploadClient UploadClient { get; set; }
    public List<GameClient> GameClients { get; set; } = new();
    
    public readonly AuthActions AuthActions;
    public readonly UploadActions UploadActions;
    public readonly GameActions GameActions;

    public NetworkService(ILogger<NetworkService> logger, IOptions<NetworkOptions> networkOptions, ICharacterService characterService)
    {
        _logger = logger;
        _characterService = characterService;
        Options = networkOptions.Value;

        AuthActions = new AuthActions(GameClients);
        UploadActions = new UploadActions();
        GameActions = new GameActions(GameClients, _characterService, this);
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

        AuthClient = new AuthClient(this);
        GameActions.AuthClient = AuthClient;
    }

    public void CreateUploadClient()
    {
        if (UploadClient != null)
        {
            _logger.LogWarning("Client already exists. Skipping creation");
            return;
        }

        UploadClient = new UploadClient(this);
    }

    public GameClient CreateGameClient(Socket socket)
    {
        var client = new GameClient(socket, this);
        
        client.CreateClientConnection();
        AuthClient.SetAffectedGameClient(client);

        return client;
    }

}