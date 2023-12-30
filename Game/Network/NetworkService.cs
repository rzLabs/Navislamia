using System.Collections.Generic;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Game.DataAccess.Repositories.Interfaces;
using Navislamia.Game.Network.Clients;
using Navislamia.Game.Network.Interfaces;
using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Packets.Interfaces;
using Navislamia.Game.Services;

namespace Navislamia.Game.Network;

// TODO: Poll connections by configuration interval
// TODO: Disconnect/Destroy

public class NetworkService : INetworkService
{
    private readonly ILogger<NetworkService> _logger;
    public readonly ICharacterService CharacterService;
    public readonly IBannedWordsRepository BannedWordsRepository;
    public readonly NetworkOptions NetworkOptions;
    public readonly ServerOptions ServerOptions;

    public AuthClient AuthClient { get; set; }
    
    public UploadClient UploadClient { get; set; }
    
    public Dictionary<string, GameClient> UnauthorizedGameClients { get; set; } = new();

    public Dictionary<string, GameClient> AuthorizedGameClients { get; set; } = new();

    public NetworkService(ILogger<NetworkService> logger, IOptions<NetworkOptions> networkOptions, 
        ICharacterService characterService, IBannedWordsRepository bannedWordsRepository,
        IOptions<ServerOptions> serverOptions)
    {
        _logger = logger;
        CharacterService = characterService;
        BannedWordsRepository = bannedWordsRepository;
        NetworkOptions = networkOptions.Value;
        ServerOptions = serverOptions.Value;
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

        return client;
    }

}