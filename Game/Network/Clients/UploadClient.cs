using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Packets.Enums;
using Navislamia.Game.Network.Packets.Interfaces;
using Navislamia.Game.Network.Packets.Upload;
using Serilog;

namespace Navislamia.Game.Network.Clients;

public class UploadClient : Client
{
    private readonly ILogger _logger = Log.ForContext<UploadClient>();
    
    public bool Ready { get; set; }
    
    public UploadClient(NetworkService networkService) : base(networkService, ClientType.Upload)
    {       
        CreateClientConnection(networkService.NetworkOptions.Upload.Ip, networkService.NetworkOptions.Upload.Port);
    }

    private void CreateClientConnection(string ip, int port)
    {
        if (!IPAddress.TryParse(ip, out var ipParsed))
        {
            _logger.Error("Failed to parse ip {ip} for Upload", ip);
            return;
        }
        
        var uploadEndpoint = new IPEndPoint(ipParsed, port);
        var uploadSock = new Socket(uploadEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        uploadSock.Connect(uploadEndpoint.Address, uploadEndpoint.Port);

        Connection = new Connection(uploadSock)
        {
            OnDataSent = OnDataSent,
            OnDataReceived = OnDataReceived,
            OnDisconnected = OnDisconnect
        };
        Connection.Start();
    }

    public override void SendMessage(IPacket msg)
    {
        _logger.Debug("{name} ({id}) Length: {length} sent to {clientTag}", msg.StructName, msg.Id, msg.Length, ClientTag);

        base.SendMessage(msg);
    }

    public override void OnDataReceived(int bytesReceived)
    {
        var remainingData = bytesReceived;

        while (remainingData > Marshal.SizeOf<Header>())
        {
            var header = new Header(Connection.Peek(Marshal.SizeOf<Header>()));
            var isValidMsg = header.Checksum == header.CalculateChecksum();

            if (!isValidMsg)
            {
                Connection.Disconnect();
                throw new Exception($"Invalid Message received from {ClientTag} !!!");
            }

            var msgBuffer = Connection.Read((int)header.Length);

            remainingData -= msgBuffer.Length;

            // Check for packets that haven't been defined yet (development)
            if (!Enum.IsDefined(typeof(UploadPackets), header.ID))
            {
                _logger.Debug("Undefined packet ID: {id} Length: {length}) received from {clientTag}", header.ID,
                    header.Length, ClientTag);
                continue;
            }

            IPacket msg = header.ID switch
            {
                (ushort)UploadPackets.TS_US_LOGIN_RESULT => new Packet<TS_US_LOGIN_RESULT>(msgBuffer),

                _ => throw new Exception("Unknown Packet Type")
            };
            
            _logger.Debug("{name}({id}) Length: {length} received from {clientTag}", msg.StructName, msg.Id,
                msg.Length, ClientTag);

            Actions.Execute(this, msg);
        }
    }
}