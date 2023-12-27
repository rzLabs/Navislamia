using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Navislamia.Game.DataAccess.Repositories.Interfaces;
using Navislamia.Game.Models.Arcadia.Enums;
using Navislamia.Game.Network.Entities.Actions;
using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Packets.Enums;
using Navislamia.Game.Services;
using Navislamia.Network.Packets;
using Serilog;

namespace Navislamia.Game.Network.Entities;

public class GameClient : Client, IDisposable
{
    private readonly ILogger _logger = Log.ForContext<GameClient>();
    
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
    public bool Authorized { get; set; }
    public int MaxConnections { get; set; } // to avoid injecting options into the client itself, i pass it through the service, find a 

    private readonly GameActions _actions;
    private AuthClient AuthClient { get; set; }

    private readonly ICharacterService _characterService;
    
    public GameClient(Socket socket, string cipherKey, int maxConnections, ICharacterService characterService, 
        AuthClient authClient, List<GameClient> authorizedClients, GameActions gameActions)
    {
        AuthClient = authClient;
        Type = ClientType.Game;
        Authorized = false;
        Connection = new CipherConnection(socket, cipherKey);

        _characterService = characterService;
        MaxConnections = maxConnections;
        _actions = gameActions;
    }
    
    public void CreateClientConnection()
    {
        ClientTag = $"{Type} Server @{Connection.RemoteIp}:{Connection.RemotePort}";
        Connection.OnDataSent = OnDataSent;
        Connection.OnDataReceived = OnDataReceived;
        Connection.OnDisconnected = OnDisconnect;
        Connection.Start();;
    }

    public override void OnDisconnect()
    {
        var packet = new Packet<TS_GA_CLIENT_LOGOUT>((ushort)AuthPackets.TS_GA_CLIENT_LOGOUT, new TS_GA_CLIENT_LOGOUT(AccountName, (uint)ContinuousPlayTime));
        AuthClient.SendMessage(packet);
        Authorized = false;
        Dispose();
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
                _logger.Warning(
                    "Partial packet received from {clientTag} !!! ID: {id} Length: {length} Available Data: {remaining}",
                    ClientTag, header.ID, header.Length, remainingData);
                Console.WriteLine($"Partial packet received from {ClientTag}");
                return;
            }

            if (!isValidMsg)
            {
                _logger.Error("Invalid Message received from {clientTag} !!!", ClientTag);
                Connection.Disconnect();
                throw new Exception($"Invalid Message recieved from {ClientTag}");
            }

            var msgBuffer = Connection.Read((int)header.Length);

            remainingData -= msgBuffer.Length;

            // Check for packets that haven't been defined yet (development)
            if (!Enum.IsDefined(typeof(GamePackets), header.ID))
            {
                _logger.Debug("Undefined packet ID: {id} Length: {length}) received from {clientTag}", header.ID, header.Length, ClientTag);
                continue;
            }

            // TM_NONE is a dummy packet sent by the clientService for...."reasons"
            if (header.ID == (ushort)GamePackets.TM_NONE)
            {
                _logger.Debug("{name}({id}) Length: {length} received from {clientTag}", "TM_NONE", header.ID, header.Length, ClientTag);
                continue;
            }

            IPacket msg = header.ID switch
            {
                //(ushort)GamePackets.TM_NONE => null,
                (ushort)GamePackets.TM_CS_VERSION => new Packet<TM_CS_VERSION>(msgBuffer),
                (ushort)GamePackets.TS_CS_CHARACTER_LIST => new Packet<TS_CS_CHARACTER_LIST>(msgBuffer),
                (ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH => new Packet<TM_CS_ACCOUNT_WITH_AUTH>(msgBuffer),
                (ushort)GamePackets.TS_CS_REPORT => new Packet<TS_CS_REPORT>(msgBuffer),

                _ => throw new Exception("Unknown Packet Type")
            };

            _logger.Debug("{name} ({id}) Length: {length} received from {clientTag}", msg.StructName, msg.ID, msg.Length, ClientTag);

            Execute(msg);
        }
    }

    private void Execute(IPacket packet)
    {
        Task.Run(() => _actions.Execute(this, packet));
    }

    public void Dispose()
    {
        if (Connection != null)
        {
            Connection.Disconnect();
            Connection = null;
        }
        
        GC.SuppressFinalize(this);
    }
}