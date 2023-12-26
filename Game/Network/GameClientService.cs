using System.Collections.Generic;
using System.Net.Sockets;
using Configuration;
using Microsoft.Extensions.Options;
using Navislamia.Game.Network.Entities;
using Navislamia.Game.Network.Interfaces;

namespace Navislamia.Game.Network;

public class GameClientService : IGameClientService
{
    private readonly NetworkOptions _networkOptions;
    private List<GameClient> Clients { get; set; } = new();
    
    public GameClientService(IOptions<NetworkOptions> networkOptions)
    {
        _networkOptions = networkOptions.Value;
    }
    
    public GameClient CreateGameClient(Socket socket)
    {
        var client = new GameClient(socket, _networkOptions.CipherKey, _networkOptions.MaxConnections);
        client.CreateClientConnection();
        Clients.Add(client);

        return client;
    }
}