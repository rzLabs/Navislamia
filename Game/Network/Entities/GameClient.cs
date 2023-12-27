using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Navislamia.Game.Models.Arcadia.Enums;
using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Packets.Enums;
using Navislamia.Game.Services;
using Navislamia.Network.Packets;
using Serilog;

namespace Navislamia.Game.Network.Entities;

public class GameClient : Client, IDisposable
{
    private readonly ILogger _logger = Log.ForContext<UploadClient>();
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
    
    private readonly Dictionary<ushort, Action<GameClient, IPacket>> _actions = new();
    public List<Client> UnauthorizedClients { get; set; } = new();
    public List<Client> AuthorizedClients { get; set; } = new();
    private AuthClient AuthClient { get; set; }
    
    private readonly ICharacterService _characterService;
    
    public GameClient(Socket socket, string cipherKey, int maxConnections, ICharacterService characterService, 
        AuthClient authClient)
    {
        AuthClient = authClient;
        Type = ClientType.Game;
        Authorized = false;
        Connection = new CipherConnection(socket, cipherKey);
        MaxConnections = maxConnections;
        _characterService = characterService;
        
        _actions.Add((ushort)GamePackets.TM_CS_VERSION, OnVersion);
        _actions.Add((ushort)GamePackets.TS_CS_REPORT, OnReport);
        _actions.Add((ushort)GamePackets.TS_CS_CHARACTER_LIST, OnCharacterList);
        _actions.Add((ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH, OnAccountWithAuth);
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

            Execute(this, msg);
        }
    }
    
    public void Execute(GameClient client, IPacket packet)
    {
        if (_actions.TryGetValue(packet.ID, out var action))
        {
            action?.Invoke(client, packet);
        }
    }

    private void OnVersion(GameClient client, IPacket packet)
    {
        // TODO: properly implement this action
    }

    private void OnReport(GameClient client, IPacket packet)
    {
        // TODO: implement me
    }

    private void OnCharacterList(GameClient client, IPacket packet)
    {
        var message = packet.GetDataStruct<TS_CS_CHARACTER_LIST>();
        var characters = _characterService.GetCharactersByAccountName(message.Account, true);
        var lobbyCharacters = new List<LobbyCharacterInfo>();

        foreach (var character in characters)
        {
            var characterLobbyInfo = new LobbyCharacterInfo
            {
                Level = character.Lv,
                Job = (int)character.CurrentJob,
                JobLevel = character.Jlv,
                ExpPercentage = 0, // TODO: needs to be done by getting values from LevelResourceRepository
                HP = character.Hp,
                MP = character.Mp,
                Permission = character.Permission,
                IsBanned = 0,
                Name = character.CharacterName,
                SkinColor = (uint)character.SkinColor,
                Sex = character.Sex,
                Race = character.Race,
                ModelId = character.Models,
                HairColorIndex = character.HairColorIndex,
                HairColorRGB = (uint)character.HairColorRgb,
                HideEquipFlag = (uint)character.HideEquipFlag,
                TextureID = character.TextureId,
                CreateTime = character.CreatedOn.ToString("yyyy/MM/dd"),
                DeleteTime = character.DeletedOn?.ToString("yyyy/MM/dd") ?? "9999/12/01",
            };

            foreach (var item in  character.Items.Where(i => i.WearInfo != ItemWearType.None))
            {
                characterLobbyInfo.WearInfo[(int)item.WearInfo] = (int)item.ItemResourceId;
                characterLobbyInfo.WearItemEnhanceInfo[(int)item.WearInfo] = (int)item.Enhance;
                characterLobbyInfo.WearItemLevelInfo[(int)item.WearInfo] = (int)item.Level;
                characterLobbyInfo.WearItemElementalType[(int)item.WearInfo] = (char)item.ElementalEffectType;
            }

            lobbyCharacters.Add(characterLobbyInfo);
        }

        SendCharacterList(client, lobbyCharacters);
    }
    
    private void SendCharacterList(GameClient client, List<LobbyCharacterInfo> characterList)
    {
        var charCount = (ushort)characterList.Count;

        var packetStructLength = Marshal.SizeOf<TS_SC_CHARACTER_LIST>();
        var lobbyCharacterStructLength = Marshal.SizeOf<LobbyCharacterInfo>();
        var lobbyCharacterBufferLength = lobbyCharacterStructLength * characterList.Count;

        var data = new TS_SC_CHARACTER_LIST(0, 0, charCount);
        var packet = new Packet<TS_SC_CHARACTER_LIST>(2004, data, packetStructLength + lobbyCharacterBufferLength);

        var charInfoOffset = Marshal.SizeOf<Header>() + packetStructLength;

        foreach (var character in characterList)
        {
            Buffer.BlockCopy(character.StructToByte(), 0, packet.Data, charInfoOffset, lobbyCharacterStructLength);

            charInfoOffset += lobbyCharacterStructLength;
        }
        
        client.Connection.Send(packet.Data);
    }
    
    private void OnAccountWithAuth(GameClient client, IPacket packet)
    {
        var message = packet.GetDataStruct<TM_CS_ACCOUNT_WITH_AUTH>();
        var connMax = client.MaxConnections;
        var loginInfo = new Packet<TS_GA_CLIENT_LOGIN>((ushort)AuthPackets.TS_GA_CLIENT_LOGIN,
            new TS_GA_CLIENT_LOGIN(message.Account, message.OneTimePassword));

        if (AuthorizedClients.Count > connMax)
        {
            client.SendResult(packet.ID, (ushort)ResultCode.LimitMax);
        }

        if (string.IsNullOrEmpty(client.AccountName))
        {
            if (client.Authorized)
            {
                client.SendResult(packet.ID, (ushort)ResultCode.AccessDenied);
            }
        }
        
        AuthClient.SendMessage(loginInfo);
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