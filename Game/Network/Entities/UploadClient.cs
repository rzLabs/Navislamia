using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Packets.Enums;

namespace Navislamia.Game.Network.Entities;

public class UploadClient : Client
{
    private readonly Dictionary<ushort, Action<UploadClient, IPacket>> _actions = new();

    public UploadClient()
    {
        Type = ClientType.Upload;
        _actions[(ushort)UploadPackets.TS_US_LOGIN_RESULT] = OnLoginResult;
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

            Execute(this, msg);
        }
    }
    
    public void Execute(UploadClient client, IPacket packet)
    {
        if (!_actions.TryGetValue(packet.ID, out var action))
        {
            return;
        }
            
        action?.Invoke(client, packet);
    }

    public void OnLoginResult(UploadClient client, IPacket packet)
    {
        var msg = packet.GetDataStruct<TS_US_LOGIN_RESULT>();

        if (msg.Result > 0)
        {
            // _logger.LogError("Failed to register to the Auth Server!");
            throw new Exception();

        }

        // _logger.LogDebug("Successfully registered to the Upload Server!");
        Console.WriteLine("Successfully registered to the Upload Server!");
    }
}