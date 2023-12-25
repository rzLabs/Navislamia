using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Navislamia.Game.Models.Arcadia.Enums;
using Navislamia.Game.Network.Entities;
using Navislamia.Game.Network.Interfaces;
using Navislamia.Game.Services;
using Navislamia.Network.Packets;

namespace Navislamia.Game.Network.Packets.Actions;

public class GameActionService : IGameActionService
{
    private readonly NetworkOptions _networkOptions;
    private readonly ILogger<GameActionService> _logger;
    private readonly ICharacterService _characterService;

    private readonly IBaseClientService _baseClientService;

    public GameActionState State { get; set; }

    private readonly Dictionary<ushort, Action<ClientEntity, IPacket>> _actions = new();

    public GameActionService(IOptions<NetworkOptions> networkOptions, 
        ICharacterService characterService, ILogger<GameActionService> logger, IBaseClientService baseClientService)
    {
        _networkOptions = networkOptions.Value;
        _characterService = characterService;
        _logger = logger;
        _baseClientService = baseClientService;

        _actions.Add((ushort)GamePackets.TM_CS_VERSION, OnVersion);
        _actions.Add((ushort)GamePackets.TS_CS_REPORT, OnReport);
        _actions.Add((ushort)GamePackets.TS_CS_CHARACTER_LIST, OnCharacterList);
        _actions.Add((ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH, OnAccountWithAuth);
    }

    public void Execute(ClientEntity client, IPacket packet)
    {
        if (!_actions.TryGetValue(packet.ID, out var action))
        {
            return;
        }
        
        action?.Invoke(client, packet);
    }

    private void OnVersion(ClientEntity client, IPacket packet)
    {
        // TODO: properly implement this action
    }

    private void OnReport(ClientEntity client, IPacket packet)
    {
        // TODO: implement me
    }

    private void OnCharacterList(ClientEntity client, IPacket packet)
    {
        var message = packet.GetDataStruct<TS_CS_CHARACTER_LIST>();
        var characters = _characterService.GetCharactersByAccountName(message.Account, true);
        var lobbyCharacters = new List<LobbyCharacterInfoEntity>();

        foreach (var character in characters)
        {
            var characterLobbyInfo = new LobbyCharacterInfoEntity
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
    
    private void SendCharacterList(ClientEntity client, List<LobbyCharacterInfoEntity> characterList)
    {
        var charCount = (ushort)characterList.Count;

        var packetStructLength = Marshal.SizeOf<TS_SC_CHARACTER_LIST>();
        var lobbyCharacterStructLength = Marshal.SizeOf<LobbyCharacterInfoEntity>();
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

    private void OnAccountWithAuth(ClientEntity client, IPacket packet)
    {
        var message = packet.GetDataStruct<TM_CS_ACCOUNT_WITH_AUTH>();
        var connMax = _networkOptions.MaxConnections;
        var loginInfo = new Packet<TS_GA_CLIENT_LOGIN>((ushort)AuthPackets.TS_GA_CLIENT_LOGIN,
            new TS_GA_CLIENT_LOGIN(message.Account, message.OneTimePassword));

        if (State.AuthorizedClients.Count > connMax)
        {
            SendResult(client, packet.ID, (ushort)ResultCode.LimitMax);
        }

        if (string.IsNullOrEmpty(client.ConnectionData.AccountName))
        {
            if (!client.IsAuthorized)
            {
                SendResult(client, packet.ID, (ushort)ResultCode.AccessDenied);
            }
            
            State.UnauthorizedClients.Add(client);
        }
        
        //auth client should send this
        _baseClientService.SendMessage(client, loginInfo);
    }
    
    public void SendResult(ClientEntity client, ushort id, ushort result, int value = 0)
    {
        var message = new Packet<TS_SC_RESULT>((ushort)GamePackets.TM_SC_RESULT, new TS_SC_RESULT(id, result, value));
        SendMessage(client, message);
    }
    
    public void SendMessage(ClientEntity client, IPacket msg)
    {
        client.Connection.Send(msg.Data);
        _logger.LogDebug("{name} ({id}) Length: {length} sent to {clientTag}", 
            msg.StructName, msg.ID, msg.Length, client.Type);
    }
}