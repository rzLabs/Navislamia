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

public class UploadClientService : BaseClientService, IUploadClientService 
{
    private readonly ILogger<UploadClientService> _logger;
    private readonly IUploadActionService _uploadActionService;
    private readonly NetworkOptions _networkOptions;
    private ClientEntity _uploadClient;

    private string _clientTag;

    public UploadClientService(IUploadActionService uploadActionService, IOptions<NetworkOptions> networkOptions, 
        ILogger<UploadClientService> logger)
    {
        _networkOptions = networkOptions.Value;
        _uploadActionService = uploadActionService;
        _logger = logger;
    }
    
    public void CreateUploadClient()
    {
        _uploadClient = Clients.FirstOrDefault(c => c.Type == ClientType.Upload);
        if (_uploadClient != null)
        {
            _logger.LogWarning("UploadClient already exists. Skipping creation");
            return;
        }

        _uploadClient = new ClientEntity
        {
            Id = Guid.NewGuid(),
            Type = ClientType.Upload
        };

        CreateClientConnection();
    }
    
    public ClientEntity GetClient()
    {
        return _uploadClient;
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
            _logger.LogError($"Failed to parse auth ip: {ip}");
            throw new Exception();

        }
        
        var authEndPoint = new IPEndPoint(ip, _networkOptions.Auth.Port);
        var authSock = new Socket(authEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        authSock.Connect(authEndPoint.Address, authEndPoint.Port);

        _uploadClient.Connection = new Connection(authSock);
        _uploadClient.Details.Ip = _uploadClient.Connection.RemoteIp;
        _uploadClient.Details.Port = _uploadClient.Connection.RemotePort;
        _uploadClient.ClientTag = $"{_uploadClient.Type} Server @{_uploadClient.Details.Ip}:{_uploadClient.Details.Port}";

        AddClient(_uploadClient);
        StartClient(_uploadClient.Id);
    }

    public override void OnDataReceived(int bytesReceived)
    {
        var remainingData = bytesReceived;

        while (remainingData > Marshal.SizeOf<Header>())
        {
            var header = new Header(_uploadClient.Connection.Peek(Marshal.SizeOf<Header>()));
            var isValidMsg = header.Checksum == Header.CalculateChecksum(header);

            if (!isValidMsg)
            {
                _logger.LogError("Invalid Message received from {clientTag} !!!", _clientTag);

                _uploadClient.Connection.Disconnect();

                return;
            }

            var msgBuffer = _uploadClient.Connection.Read((int)header.Length);

            remainingData -= msgBuffer.Length;

            // Check for packets that haven't been defined yet (development)
            if (!Enum.IsDefined(typeof(UploadPackets), header.ID))
            {
                _logger.LogDebug("Undefined packet ID: {id} Length: {length}) received from {clientTag}", header.ID, header.Length, _clientTag);
                continue;
            }

            IPacket msg = header.ID switch
            {
                (ushort)UploadPackets.TS_US_LOGIN_RESULT => new Packet<TS_US_LOGIN_RESULT>(msgBuffer),

                _ => throw new Exception("Unknown Packet Type")
            };

            _logger.LogDebug("{name}({id}) Length: {length} received from {clientTag}", msg.StructName, msg.ID, msg.Length, _clientTag);

            _uploadActionService.Execute(this, msg);
        }
    }
}