using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Packets.Auth;
using Navislamia.Game.Network.Packets.Enums;

namespace Navislamia.Game.Network.Entities;

public class AuthClient : Client
{
    private readonly Dictionary<ushort, Action<AuthClient, IPacket>> _actions = new();
    private List<GameClient> GameClients { get; set; }

    public GameClient AffectedGameClient { get;set; }
    
    public AuthClient(List<GameClient> gameClients)
    {
        Type = ClientType.Auth;
        GameClients = gameClients;
        
        _actions[(ushort)AuthPackets.TS_AG_LOGIN_RESULT] = OnLoginResult;
        _actions[(ushort)AuthPackets.TS_AG_CLIENT_LOGIN] = OnAuthClientLoginResult;
        _actions[(ushort)AuthPackets.TS_GA_CLIENT_LOGOUT] = OnClientLogout;
    }

    public void SetAffectedGameClient(GameClient client)
    {
        AffectedGameClient = client;
    }

    public void OnClientLogout(AuthClient client, IPacket packet)
    {
        if (AffectedGameClient.LoggedIn)
        {
            // TODO AffectedGameClient.ContinuousPlayTime 
            AffectedGameClient.Connection.Disconnect();
        }
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
                (ushort)AuthPackets.TS_GA_CLIENT_LOGOUT => new Packet<TS_GA_CLIENT_LOGOUT>(msgBuffer),
                
                _ => throw new Exception("Unknown Packet Type")
            };

            // _logger.LogDebug("{name}({id}) Length: {length} received from {clientTag}", msg.StructName, msg.ID,
            //     msg.Length, ClientTag);

            Execute(this, msg);
        }
        
    }
    
    public void Execute(AuthClient client, IPacket packet)
    {
        if (!_actions.TryGetValue(packet.ID, out var action))
        {
            return;
        }

        action?.Invoke(client, packet);
    }
    
    private void OnLoginResult(AuthClient client, IPacket packet)
    {
        var msg = packet.GetDataStruct<TS_AG_LOGIN_RESULT>();

        if (msg.Result > 0)
        {
            throw new Exception("Failed to register to the Auth Server!");
        }
        
        Console.WriteLine("Successfully registered to the Auth Server!");
        // _logger.LogDebug("Successfully registered to the Auth Server!");
    }

    // Only gameclient is affected but its an auth packet so the action should be performed here.
    // Thats why the first parameter is authclient
    private void OnAuthClientLoginResult(AuthClient authClient, IPacket packet)
    {
        var userLogin = packet.GetDataStruct<TS_AG_CLIENT_LOGIN>();
        // Check if the game networkService connection is queued in AuthAccounts
        if (AffectedGameClient.LoggedIn)
        {
            // _logger.LogError("Account register failed for: {accountName}", agClientLogin.Account);
            // TODO: SendLogoutToAuth user is already islogged in, wrong credentials etc -> send logout to auth
            userLogin.Result = (ushort)ResultCode.AccessDenied;
        }
        else
        {
            if (userLogin.Result == (ushort)ResultCode.Success)
            {
                AffectedGameClient.AccountName = userLogin.Account;
                AffectedGameClient.AccountId = userLogin.AccountID;
                AffectedGameClient.AuthVerified = true;
                AffectedGameClient.PcBangMode = userLogin.PcBangMode;
                AffectedGameClient.EventCode = userLogin.EventCode;
                AffectedGameClient.Age = userLogin.Age;
                AffectedGameClient.ContinuousPlayTime = userLogin.ContinuousPlayTime;
                AffectedGameClient.ContinuousLogoutTime = userLogin.ContinuousLogoutTime;
            }
            AffectedGameClient.LoggedIn = true;
        }

        AffectedGameClient.SendResult((ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH, userLogin.Result);
    }
}