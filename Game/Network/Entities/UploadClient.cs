using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Packets.Actions;
using Navislamia.Game.Network.Packets.Enums;

namespace Navislamia.Game.Network.Entities;

public class UploadClient : Client
{
    private readonly UploadActionService _action = new();
    
    public UploadClient()
    {
        Type = ClientType.Upload;
        IsAuthorized = true;
    }
    
    public void CreateClientConnection(string ip, int port)
    {
        if (!IPAddress.TryParse(ip, out var ipParsed))
        {
            throw new Exception($"Failed to parse upload ip: {ip}");
        }
        
        var uploadEndpoint = new IPEndPoint(ipParsed, port);
        var uploadSock = new Socket(uploadEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        uploadSock.Connect(uploadEndpoint.Address, uploadEndpoint.Port);

        Connection = new Connection(uploadSock);
        ClientTag = $"{Type} Server @{Connection.LocalIp}:{Connection.LocalPort}";

        Connection.OnDataSent = OnDataSent;
        Connection.OnDataReceived = OnDataReceived;
        Connection.OnDisconnected = OnDisconnect;
        Connection.Start();;
    }
    
    public override void OnDataReceived(int bytesReceived)
    {
        var remainingData = bytesReceived;

        while (remainingData > Marshal.SizeOf<Header>())
        {
            var header = new Header(Connection.Peek(Marshal.SizeOf<Header>()));
            var isValidMsg = header.Checksum == Header.CalculateChecksum(header);

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
                // _logger.LogDebug("Undefined packet ID: {id} Length: {length}) received from {clientTag}", header.ID,
                    // header.Length, _uploadClientHandler.Client.ClientTag);
                continue;
            }

            IPacket msg = header.ID switch
            {
                (ushort)UploadPackets.TS_US_LOGIN_RESULT => new Packet<TS_US_LOGIN_RESULT>(msgBuffer),

                _ => throw new Exception("Unknown Packet Type")
            };
            
            // _logger.LogDebug("{name}({id}) Length: {length} received from {clientTag}", msg.StructName, msg.ID,
            //     msg.Length, _uploadClientHandler.Client.ClientTag);

            _action.Execute(this, msg);
        }
    }
}