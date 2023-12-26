using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Packets.Actions;
using Navislamia.Game.Network.Packets.Enums;
using Navislamia.Network.Packets;

namespace Navislamia.Game.Network.Entities;

public class GameClient : Client
{
    public string AccountName { get; set; }
    public List<string> CharacterList { get; set; } = new();
    // TODO: StructPlayer Player;
    public int AccountId { get; set; }
    public int Version { get; set; }
    public float LastReadTime { get; set; }
    public bool AuthVerified { get; set; }
    public byte PcBangMode { get; set; }
    public int EventCode { get; set; }
    public int Age { get; set; }
    public int AgeLimitFlags { get; set; }
    public float ContinuousPlayTime { get; set; }
    public float ContinuousLogoutTime { get; set; }
    public float LastContinuousPlayTimeProcTime;
    public string NameToDelete { get; set; }
    public bool StorageSecurityCheck { get; set; } = false;
    public int MaxConnections { get; set; } // to avoid injecting options into the client itself, i pass it through the service, find a 
    
    private readonly GameActionService _action = new();

    public GameClient(Socket socket, string cipherKey, int maxConnections)
    {
        Type = ClientType.Game;
        IsAuthorized = false;
        Connection = new CipherConnection(socket, cipherKey);
        MaxConnections = maxConnections;
        
        _action.State.UnauthorizedClients.Add(this);
    }
    
    public void CreateClientConnection()
    {
        ClientTag = $"{Type} Server @{Connection.LocalIp}:{Connection.LocalPort}";
        Connection.OnDataSent = OnDataSent;
        Connection.OnDataReceived = OnDataReceived;
        Connection.OnDisconnected = OnDisconnect;
        Connection.Start();;
    }
    
    public void SendResult(ushort id, ushort result, int value = 0)
    {
        var message = new Packet<TS_SC_RESULT>((ushort)GamePackets.TM_SC_RESULT, new TS_SC_RESULT(id, result, value));
        SendMessage(message);
    }
    
    public override void OnDataReceived(int bytesReceived)
    {
        var remainingData = bytesReceived;

        while (remainingData > Marshal.SizeOf<Header>())
        {
            var header = new Header(Connection.Peek(Marshal.SizeOf<Header>()));
            var isValidMsg = header.Checksum == Header.CalculateChecksum(header);

            if (header.Length > remainingData)
            {
                // _logger.LogWarning(
                //     "Partial packet received from {clientTag} !!! ID: {id} Length: {length} Available Data: {remaining}",
                //     client.ClientTag, header.ID, header.Length, remainingData);
                Console.WriteLine($"Partial packet received from {ClientTag}");
                return;
            }

            if (!isValidMsg)
            {
                // _logger.LogError("Invalid Message received from {clientTag} !!!", client.ClientTag);
                Connection.Disconnect();
                throw new Exception($"Invalid Message recieved from {ClientTag}");
            }

            var msgBuffer = Connection.Read((int)header.Length);

            remainingData -= msgBuffer.Length;

            // Check for packets that haven't been defined yet (development)
            if (!Enum.IsDefined(typeof(GamePackets), header.ID))
            {
                // _logger.LogDebug("Undefined packet ID: {id} Length: {length}) received from {clientTag}", header.ID, header.Length, client.ClientTag);
                continue;
            }

            // TM_NONE is a dummy packet sent by the clientService for...."reasons"
            if (header.ID == (ushort)GamePackets.TM_NONE)
            {
                // _logger.LogDebug("{name}({id}) Length: {length} received from {clientTag}", "TM_NONE", header.ID, header.Length, client.ClientTag);
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

            // _logger.LogDebug("{name} ({id}) Length: {length} received from {clientTag}", msg.StructName, msg.ID, msg.Length, client.ClientTag);

            _action.Execute(this, msg);
        }
    }
    
}