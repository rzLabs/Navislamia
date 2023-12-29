using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Configuration;
using Navislamia.Game.Models.Arcadia.Enums;
using Navislamia.Game.Network.Packets;
using Navislamia.Game.Services;
using Navislamia.Network.Packets;
using Serilog;

using Navislamia.Game.Network.Interfaces;
using Navislamia.Game.Network.Packets.Game;
using Navislamia.Game.Network.Extensions;
using Navislamia.Game.Models.Telecaster;

using Navislamia.Game.Models.Enums;

namespace Navislamia.Game.Network.Entities.Actions;

public class GameActions : IActions
{
    private readonly ILogger _logger = Log.ForContext<GameActions>();
    private readonly ICharacterService _characterService;
    private readonly NetworkService _networkService;

    private readonly Dictionary<ushort, Action<GameClient, IPacket>> _actions = new();
   
    public GameActions(NetworkService networkService)
    {
        _networkService = networkService;
        _characterService = networkService.CharacterService;

        _actions.Add((ushort)GamePackets.TM_CS_VERSION, OnVersion);
        _actions.Add((ushort)GamePackets.TM_CS_REPORT, OnReport);
        _actions.Add((ushort)GamePackets.TM_CS_CHARACTER_LIST, OnCharacterList);
        _actions.Add((ushort)GamePackets.TM_CS_CREATE_CHARACTER, OnCreateCharacter);
        _actions.Add((ushort)GamePackets.TM_CS_CHECK_CHARACTER_NAME, OnCheckCharacterName);
        _actions.Add((ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH, OnAccountWithAuth);
    }

    public void Execute(Client client, IPacket packet)
    {
        if (_actions.TryGetValue(packet.ID, out var action))
        {
            action?.Invoke(client as GameClient, packet);
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
    
    private async void OnCharacterList(GameClient client, IPacket packet)
    {
        var message = packet.GetDataStruct<TS_CS_CHARACTER_LIST>();
        var characters = await _characterService.GetCharactersByAccountNameAsync(message.Account, true);
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

    private void OnCreateCharacter(GameClient client, IPacket packet)
    {
        var _createMsg = packet.GetDataStruct<TS_CS_CREATE_CHARACTER>();

        var _formattedName = _createMsg.Info.Name.FormatName();

        var _character = new CharacterEntity() // We don't need to pass all info, most of it is safely ignored and should default when inserted to Character table
        {
            AccountName = client.ConnectionInfo.AccountName,
            CharacterName = _formattedName,
            Sex = _createMsg.Info.Sex,
            Race = _createMsg.Info.Race,
            Models = _createMsg.Info.ModelId,
            HairColorIndex = _createMsg.Info.HairColorIndex,
            TextureId = _createMsg.Info.TextureID,
            SkinColor = (int)_createMsg.Info.SkinColor
        };

        // TODO: Nexitis this method needs to return a bool
        // _characterService.CreateCharacterAsync(_character, true);

        // TODO: needs to be like
        /*
         * if (!_characterService.CreateCharacterAsync(_character, true)
         * {
         *  client.SendResult(packet.ID, (ushort)ResultCode.AccessDenied); // <--- this should never happen
         * }
         * 
         * client.SendResult(packet.ID, (ushort)ResultCode.Success);
         */
    }

    private void OnCheckCharacterName(GameClient client, IPacket packet)
    {
        var _nameMsg = packet.GetDataStruct<TS_CS_CHECK_CHARACTER_NAME>();

        if (string.IsNullOrEmpty(_nameMsg.Name))
        {
            client.SendResult(packet.ID, (ushort)ResultCode.AccessDenied);

            _logger.Debug("Character Name Check Failed! Empty Name for {clientTag} !!!", client.ClientTag);

            return;
        }

        if (!_nameMsg.Name.IsValidName(4, 18))
        {
            client.SendResult(packet.ID, (ushort)ResultCode.InvalidText);

            _logger.Debug("Character Name Check Failed! Invalid Name ({name}) for {clientTag} !!!", _nameMsg.Name, client.ClientTag);

            return;
        }

        if (_characterService.CharacterExists(_nameMsg.Name)) 
        {
            client.SendResult(packet.ID, (ushort)ResultCode.AlreadyExist);

            _logger.Debug("Character Name Check Failed! Name ({name}) already exists! for {clientTag} !!!", _nameMsg.Name, client.ClientTag);

            return;
        }

        // TODO: Nexitis we must check IsBannedWord -> Banned Words are loaded from Arcadia.dbo.BanWordResource

        _logger.Debug("Character Name Check Passed! for {clientTag}", client.ClientTag);

        client.SendResult(packet.ID, (ushort)ResultCode.Success);
    }


    private void OnAccountWithAuth(GameClient client, IPacket packet)
    {
        _logger.Debug("{clientTag} verifying with Auth Server", client.ClientTag);

        var msg = packet.GetDataStruct<TM_CS_ACCOUNT_WITH_AUTH>();
        var loginInfo = new Packet<TS_GA_CLIENT_LOGIN>((ushort)AuthPackets.TS_GA_CLIENT_LOGIN,
            new TS_GA_CLIENT_LOGIN(msg.Account, msg.OneTimePassword));

        if (_networkService.AuthorizedGameClients.Count > _networkService.Options.MaxConnections)
        {
            client.SendResult(packet.ID, (ushort)ResultCode.LimitMax);
        }

        if (string.IsNullOrEmpty(client.ConnectionInfo.AccountName))
        {
            if (_networkService.UnauthorizedGameClients.ContainsKey(msg.Account))
            {
                client.SendResult(packet.ID, (ushort)ResultCode.AccessDenied);
                return;
            }

            _networkService.UnauthorizedGameClients.Add(msg.Account, client);
        }
    
        _networkService.AuthClient.SendMessage(loginInfo);
    }
}