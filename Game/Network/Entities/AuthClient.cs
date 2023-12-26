using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Packets.Actions;
using Navislamia.Game.Network.Packets.Auth;
using Navislamia.Game.Network.Packets.Enums;

namespace Navislamia.Game.Network.Entities;

public class AuthClient : Client
{
    private readonly AuthActionService _action = new();
    
    public AuthClient()
    {
        Type = ClientType.Auth;
        IsAuthorized = true;
    }
    
    public void CreateClientConnection(string ip, int port)
    {
        if (!IPAddress.TryParse(ip, out var ipParsed))
        {
            throw new Exception($"Failed to parse auth ip: {ip}");
        }
        
        var authEndPoint = new IPEndPoint(ipParsed, port);
        var authSock = new Socket(authEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        authSock.Connect(authEndPoint.Address, authEndPoint.Port);

        Connection = new Connection(authSock);
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
            if (!Enum.IsDefined(typeof(AuthPackets), header.ID))
            {
                // _logger.LogDebug("Undefined packet ID: {id} Length: {length}) received from {clientTag}", header.ID,
                //     header.Length, ClientTag);
                continue;
            }

            IPacket msg = header.ID switch
            {
                (ushort)AuthPackets.TS_AG_LOGIN_RESULT => new Packet<TS_AG_LOGIN_RESULT>(msgBuffer),
                (ushort)AuthPackets.TS_AG_CLIENT_LOGIN => new Packet<TS_AG_CLIENT_LOGIN>(msgBuffer),

                _ => throw new Exception("Unknown Packet Type")
            };

            // _logger.LogDebug("{name}({id}) Length: {length} received from {clientTag}", msg.StructName, msg.ID,
            //     msg.Length, ClientTag);

            _action.Execute(this, msg);
        }
    }
}