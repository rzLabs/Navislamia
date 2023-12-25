using Navislamia.Network.Packets;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using System.Net;
using Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Entities;
using Navislamia.Game.Network.Interfaces;
using Navislamia.Game.Network.Packets.Enums;

namespace Navislamia.Game.Network;

public class GameClientService : BaseClientService, IGameClientService
{
    private readonly ILogger<GameClientService> _logger;
    private readonly IGameActionService _gameActionService;
    private readonly NetworkOptions _networkOptions;

    private readonly List<ClientEntity> _unauthorizedGameClients = new();
    private readonly List<ClientEntity> _authorizedGameClients = new();

    private readonly string _cipherKey;
    
    public GameClientService(IGameActionService gameActionService, IOptions<NetworkOptions> networkOptions, 
        ILogger<GameClientService> logger)
    {
        _logger = logger;
        _networkOptions = networkOptions.Value;
        _gameActionService = gameActionService;

        _cipherKey = _networkOptions.CipherKey;
    }

    public bool IsAuthorized(string accountName)
    {
        return _authorizedGameClients.Any(c => c.AccountName == accountName);
    }
    
    public bool IsUnauthorized(string accountName)
    {
        return _unauthorizedGameClients.Any(c => c.AccountName == accountName);
    }
    
    public void AuthorizeClient(string accountName, ClientEntity client)
    {
        if (IsAuthorized(accountName))
        {
            return;
        }

        _unauthorizedGameClients.Remove(client);
        _authorizedGameClients.Add(client);
    }
    
    public void SendResult(ClientEntity client, ushort id, ushort result, int value = 0)
    {
        var message = new Packet<TS_SC_RESULT>((ushort)GamePackets.TM_SC_RESULT, new TS_SC_RESULT(id, result, value));
        SendMessage(client, message);
    }
   
    public void UnAuthorizeClient(string accountName, ClientEntity client)
    {
        if (IsUnauthorized(accountName))
        {
            return;
        }
        
        _authorizedGameClients.Remove(client);
        _unauthorizedGameClients.Add(client);
    }
    
    public ClientEntity CreateGameClient(Socket socket)
    {
        var gameClient = new ClientEntity
        {
            Id = Guid.NewGuid(),
            Type = ClientType.Game
        };

        CreateClientConnection(gameClient, socket);
        return gameClient;
    }

    public void RemoveGameClient(ClientEntity client)
    {
        throw new NotImplementedException();
    }

    public List<ClientEntity> GetAuthorizedClients()
    {
        return _authorizedGameClients;
    }

    public IEnumerable<ClientEntity> GetUnauthorizedClients()
    {
        return _unauthorizedGameClients;
    }

    private void CreateClientConnection(ClientEntity client, Socket socket)
    {
        client.Connection = new CipherConnection(socket, _cipherKey);
        client.Details.Ip = client.Connection.RemoteIp;
        client.Details.Port = client.Connection.RemotePort;
        client.ClientTag = $"{client.Type} Server @{client.Details.Ip}:{client.Details.Port}";

        AddClient(client);
        StartClient(client.Id);
    }

    public override void OnDataReceived(string accountName, int bytesReceived)
    {
        var client = _authorizedGameClients.FirstOrDefault(g => g.AccountName == accountName);

        if (client == null)
        {
            _logger.LogError("Data recieved from accountName {name} but no corresponding client was found. Skipping.", accountName);
            return;
        }
        
        var remainingData = bytesReceived;

        while (remainingData > Marshal.SizeOf<Header>())
        {
            var header = new Header(client.Connection.Peek(Marshal.SizeOf<Header>()));
            var isValidMsg = header.Checksum == Header.CalculateChecksum(header);

            if (header.Length > remainingData)
            {
                _logger.LogWarning(
                    "Partial packet received from {clientTag} !!! ID: {id} Length: {length} Available Data: {remaining}",
                    client.ClientTag, header.ID, header.Length, remainingData);

                return;
            }

            if (!isValidMsg)
            {
                _logger.LogError("Invalid Message received from {clientTag} !!!", client.ClientTag);

                client.Connection.Disconnect();

                return;
            }

            var msgBuffer = client.Connection.Read((int)header.Length);

            remainingData -= msgBuffer.Length;

            // Check for packets that haven't been defined yet (development)
            if (!Enum.IsDefined(typeof(GamePackets), header.ID))
            {
                _logger.LogDebug("Undefined packet ID: {id} Length: {length}) received from {clientTag}", header.ID, header.Length, client.ClientTag);
                continue;
            }

            // TM_NONE is a dummy packet sent by the clientService for...."reasons"
            if (header.ID == (ushort)GamePackets.TM_NONE)
            {
                _logger.LogDebug("{name}({id}) Length: {length} received from {clientTag}", "TM_NONE", header.ID, header.Length, client.ClientTag);

                continue;
            }

            IPacket msg = header.ID switch
            {
                // Game
                //(ushort)GamePackets.TM_NONE => null,
                (ushort)GamePackets.TM_CS_VERSION => new Packet<TM_CS_VERSION>(msgBuffer),
                (ushort)GamePackets.TS_CS_CHARACTER_LIST => new Packet<TS_CS_CHARACTER_LIST>(msgBuffer),
                (ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH => new Packet<TM_CS_ACCOUNT_WITH_AUTH>(msgBuffer),
                (ushort)GamePackets.TS_CS_REPORT => new Packet<TS_CS_REPORT>(msgBuffer),

                _ => throw new Exception("Unknown Packet Type")
            };

            _logger.LogDebug("{name} ({id}) Length: {length} received from {clientTag}", msg.StructName, msg.ID, msg.Length, client.ClientTag);

            _gameActionService.State.AuthorizedClients = _authorizedGameClients;
            _gameActionService.State.UnauthorizedClients = _unauthorizedGameClients;
            _gameActionService.Execute(client, msg);
        }
    }

    // public override void OnDisconnect(string accountName)
    // {
    //     var client = _authorizedGameClients.FirstOrDefault(c => c.AccountName == accountName);
    //     RemoveGameClient(client);
    // }
    //
    // public void RemoveGameClient(ClientEntity client)
    // {
    //     var msg = new Packet<TS_GA_CLIENT_LOGOUT>((ushort)AuthPackets.TS_GA_CLIENT_LOGOUT,
    //         new TS_GA_CLIENT_LOGOUT(client.AccountName, (uint)client.ConnectionData.ContinuousPlayTime));
    //
    //     var authClient = _authClientService.GetClient();
    //     _authClientService.SendMessage(authClient, msg);
    //     UnAuthorizeClient(client.AccountName, client);
    //
    //     _logger.LogDebug("{clientTag} disconnected!", client.ClientTag);
    // }

}