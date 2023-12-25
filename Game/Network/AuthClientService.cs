using Navislamia.Game.Network.Packets;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Navislamia.Game.Network.Entities;
using Navislamia.Game.Network.Interfaces;
using Navislamia.Game.Network.Packets.Enums;

namespace Navislamia.Game.Network;

public class AuthClientService : BaseClientService, IAuthClientService
{
    private readonly ILogger<AuthClientService> _logger;
    private readonly IAuthActionService _authActionService;
    private readonly NetworkOptions _networkOptions;

    private ClientEntity _authClient;

    public AuthClientService(IAuthActionService authActionService, IOptions<NetworkOptions> networkOptions,
        ILogger<AuthClientService> logger)
    {
        _logger = logger;
        _networkOptions = networkOptions.Value;
        _authActionService = authActionService;
    }

    public void CreateAuthClient()
    {
        _authClient = Clients.FirstOrDefault(c => c.Type == ClientType.Auth);
        if (_authClient != null)
        {
            _logger.LogWarning("AuthClient already exists. Skipping creation");
            return;
        }
        
        _authClient = new ClientEntity
        {
            Id = Guid.NewGuid(),
            Type = ClientType.Auth
        };
        
        CreateClientConnection();
    }

    public ClientEntity GetClient()
    {
        return _authClient;
    }

    private void CreateClientConnection()
    {
        if (string.IsNullOrEmpty(_networkOptions.Auth.Ip) ||  _networkOptions.Auth.Port == 0)
        {
            _logger.LogError("Invalid network auth ip! Review your configuration!");
            throw new Exception();
        }

        if (!IPAddress.TryParse(_networkOptions.Auth.Ip, out var ip))
        {
            _logger.LogError("Failed to parse auth ip: {ip}", ip);
            throw new Exception();
        }
        
        var authEndPoint = new IPEndPoint(ip, _networkOptions.Auth.Port);
        var authSock = new Socket(authEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        authSock.Connect(authEndPoint.Address, authEndPoint.Port);

        _authClient.Connection = new Connection(authSock);
        _authClient.Details.Ip = _authClient.Connection.RemoteIp;
        _authClient.Details.Port = _authClient.Connection.RemotePort;
        _authClient.ClientTag = $"{_authClient.Type} Server @{_authClient.Details.Ip}:{_authClient.Details.Port}";

        AddClient(_authClient);
        StartClient(_authClient.Id);
    }

    public override void OnDataReceived(int bytesReceived)
    {
        var remainingData = bytesReceived;

        while (remainingData > Marshal.SizeOf<Header>())
        {
            var header = new Header(_authClient.Connection.Peek(Marshal.SizeOf<Header>()));
            var isValidMsg = header.Checksum == Header.CalculateChecksum(header);

            if (!isValidMsg)
            {
                _logger.LogError("Invalid Message received from {clientTag} !!!", _authClient.ClientTag);
                _authClient.Connection.Disconnect();
                return;
            }

            var msgBuffer = _authClient.Connection.Read((int)header.Length);

            remainingData -= msgBuffer.Length;

            // Check for packets that haven't been defined yet (development)
            if (!Enum.IsDefined(typeof(AuthPackets), header.ID))
            {
                _logger.LogDebug("Undefined packet ID: {id} Length: {length}) received from {clientTag}", header.ID,
                    header.Length, _authClient.ClientTag);
                continue;
            }

            IPacket msg = header.ID switch
            {
                // Auth
                (ushort)AuthPackets.TS_AG_LOGIN_RESULT => new Packet<TS_AG_LOGIN_RESULT>(msgBuffer),
                (ushort)AuthPackets.TS_AG_CLIENT_LOGIN => new Packet<TS_AG_CLIENT_LOGIN>(msgBuffer),

                _ => throw new Exception("Unknown Packet Type")
            };

            _logger.LogDebug("{name}({id}) Length: {length} received from {clientTag}", msg.StructName, msg.ID,
                msg.Length, _authClient.ClientTag);

            _authActionService.Execute(this, msg);
        }
    }
}
