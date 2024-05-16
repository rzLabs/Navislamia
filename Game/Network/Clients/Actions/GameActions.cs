using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Navislamia.Game.Creature.Interfaces;
using Navislamia.Game.DataAccess.Entities.Enums;
using Navislamia.Game.DataAccess.Entities.Telecaster;
using Navislamia.Game.DataAccess.Repositories.Interfaces;
using Navislamia.Game.Extensions;
using Navislamia.Game.Network.Extensions;
using Navislamia.Game.Network.Interfaces;
using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Packets.Auth;
using Navislamia.Game.Network.Packets.Enums;
using Navislamia.Game.Network.Packets.Game;
using Navislamia.Game.Network.Packets.Interfaces;
using Navislamia.Game.Services;
using Serilog;

namespace Navislamia.Game.Network.Clients.Actions;

public class GameActions : IActions
{
    private readonly ILogger _logger = Log.ForContext<GameActions>();
    private readonly IPlayerService _playerService;
    private readonly ICharacterService _characterService;
    private readonly IBannedWordsRepository _bannedWordsRepository;
    private readonly NetworkService _networkService;

    private readonly Dictionary<ushort, Action<GameClient, IPacket>> _actions = new();
   
    public GameActions(NetworkService networkService)
    {
        _networkService = networkService;
        _bannedWordsRepository = networkService.BannedWordsRepository;
        _playerService = networkService.PlayerService;
        _characterService = networkService.CharacterService;

        _actions.Add((ushort)GamePackets.TM_CS_LOGIN, OnLogin);
        _actions.Add((ushort)GamePackets.TM_CS_VERSION, OnVersion);
        _actions.Add((ushort)GamePackets.TM_CS_REPORT, OnReport);
        _actions.Add((ushort)GamePackets.TM_CS_CHARACTER_LIST, OnCharacterList);
        _actions.Add((ushort)GamePackets.TM_CS_CREATE_CHARACTER, OnCreateCharacter);
        _actions.Add((ushort)GamePackets.TM_CS_DELETE_CHARACTER, OnDeleteCharacter);
        _actions.Add((ushort)GamePackets.TM_CS_CHECK_CHARACTER_NAME, OnCheckCharacterName);
        _actions.Add((ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH, OnAccountWithAuth);
    }

    public void Execute(Client client, IPacket packet)
    {
        if (_actions.TryGetValue(packet.Id, out var action))
        {
            action?.Invoke(client as GameClient, packet);
        }
    }

    private void OnLogin(GameClient client, IPacket packet)
    {
        var message = packet.GetDataStruct<TS_LOGIN>();

        if (client.Options.UseLoginLogoutDebug)
        {
            _logger.Debug("On Login: {characterName}@{accountName}", message.Name, client.ConnectionInfo.AccountName);
        }

        if (!client.ConnectionInfo.CharacterList.Contains(message.Name))
        {
            _logger.Error("Failed login attempt, account {accountName} does not contain character {characterName}", client.ConnectionInfo.AccountName, message.Name);

            client.Connection.Disconnect();

            return;
        }

        if (_playerService.GetPlayerHandle(message.Name) >= 0) 
        {
            _logger.Error("Duplicate login attempt, account {accountName}@{characterName}", client.ConnectionInfo.AccountName, message.Name);

            // TODO: Save, if login then logout 

            client.SendDisconnectDescription(DisconnectType.DuplicatedLogin);

            client.Connection.Disconnect();
        }

        client.ConnectionInfo.Player = _playerService.CreatePlayer();
        client.ConnectionInfo.Player.Connection = client.Connection;
        client.ConnectionInfo.StorageSecurityCheck = false;

        // TODO: Send urlist
        // TODO: execute DB_Login

        _logger.Fatal("On Login Not Implemented!");

        return;
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

            if (!character.Items.IsNullOrEmpty())
            {
                foreach (var item in character.Items.Where(i => i.WearInfo != ItemWearType.None))
                {
                    characterLobbyInfo.WearInfo[(int)item.WearInfo] = (int)item.ItemResourceId;
                    characterLobbyInfo.WearItemEnhanceInfo[(int)item.WearInfo] = (int)item.Enhance;
                    characterLobbyInfo.WearItemLevelInfo[(int)item.WearInfo] = (int)item.Level;
                    characterLobbyInfo.WearItemElementalType[(int)item.WearInfo] = (char)item.ElementalEffectType;
                }
            }

            lobbyCharacters.Add(characterLobbyInfo);

            client.ConnectionInfo.CharacterList.Add(character.CharacterName);
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

    private async void OnCreateCharacter(GameClient client, IPacket packet)
    {
        var createMsg = packet.GetDataStruct<TS_CS_CREATE_CHARACTER>();

        if (_characterService.CharacterCount(client.ConnectionInfo.AccountId) >= 6)
        {
            _logger.Debug("Character create failed! Limit reached! for ({accountName}) {clientTag} !!!", client.ConnectionInfo.AccountName, client.ClientTag);

            client.SendResult(packet.Id, (ushort)ResultCode.LimitMax);

            return;
        }

        var selectedArmor = createMsg.Info.WearInfo[(int)ItemWearType.Armor];

        // Set default weapon and armor ids
        int defaultArmorId;
        int defaultWeaponId;

        switch ((Race)createMsg.Info.Race)
        {
            case Race.Deva:
                defaultArmorId = selectedArmor == 602 ? 220109 : 220100;
                defaultWeaponId = 106100;
                break;

            case Race.Gaia:
                defaultArmorId = selectedArmor == 602 ? 240109 : 240100;
                defaultWeaponId = 112100;
                break;

            case Race.Asura:
                defaultArmorId = selectedArmor == 602 ? 230109 : 230100;
                defaultWeaponId = 103100;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(createMsg.Info.Race));
        }

        // We don't need to pass all info, most of it is safely ignored and should default when inserted to Character table
        var character = new CharacterEntity 
        {
            AccountId = client.ConnectionInfo.AccountId,
            AccountName = client.ConnectionInfo.AccountName,
            CharacterName = createMsg.Info.Name.FormatName(),
            Sex = createMsg.Info.Sex,
            Race = createMsg.Info.Race,
            Models = createMsg.Info.ModelId,
            HairColorIndex = createMsg.Info.HairColorIndex,
            TextureId = createMsg.Info.TextureID,
            SkinColor = (int)createMsg.Info.SkinColor,

            // Add default gear to the character
            Items = new List<ItemEntity>
            {
                new() { ItemResourceId = defaultArmorId, Level = 1, Amount = 1, Endurance = 50, WearInfo = ItemWearType.Armor, GenerateBySource = ItemGenerateSource.Basic },
                new() { ItemResourceId = defaultWeaponId, Level = 1, Amount = 1, Endurance = 50, WearInfo = ItemWearType.Weapon, GenerateBySource = ItemGenerateSource.Basic },

                // TODO: bag item id should come from config
                new() { ItemResourceId = 490001, Level = 1, Amount = 1, Endurance = 50, WearInfo = ItemWearType.BagSlot, GenerateBySource = ItemGenerateSource.Basic}
            }
        };

        var createdEntity = await _characterService.CreateCharacterAsync(character);

        if (createdEntity == null)
        {
            // Should never happen
            _logger.Error("Character create failed! for ({accountName}) {clientTag} !!!", character.AccountName, client.ClientTag);

            client.SendResult(packet.Id, (ushort)ResultCode.DBError);
        }

        _logger.Debug("Character {characterName} successfully created for ({accountName}) {clientTag}", character.CharacterName, client.ConnectionInfo.AccountName, client.ClientTag);

        client.SendResult(packet.Id, (ushort)ResultCode.Success);
    }

    private void OnDeleteCharacter(GameClient client, IPacket packet)
    {
        // Normally a player cannot request a character delete before any have been made, bad actor!
        if (client.ConnectionInfo.CharacterList.Count == 0)
        {
            client.SendDisconnectDescription(DisconnectType.AntiHack);

            client.Dispose();

            return;
        }

        var deleteMsg = packet.GetDataStruct<TS_CS_DELETE_CHARACTER>();

        // TODO: implement delete security

        // TODO: check if is guild leader (and send result AccessDenied)

        // TODO: get party (if leader, destroy. If member, leave)

        // TODO: remove own friends list entries

        // TODO: remove self from friend list of friends

        // TODO: remove denials (people blocked)

        // TODO: remove self from other players denials

        // TODO: remove self from ranking score

        // TODO: update player name to have @ at the front of it and set DeleteOn date
        
        _characterService.DeleteCharacterByNameAsync(deleteMsg.Name);
        
        // Do not call SaveChanges in any of the methods above as if anything fails the changes are not commited
        // into the database unless SaveChanges has been run. This is a protective messure to not remove too much and 
        // and risk a failure that corrupts the entities
        _characterService.SaveChanges();
        client.SendResult(packet.Id, (ushort)ResultCode.Success);
    }

    private void OnCheckCharacterName(GameClient client, IPacket packet)
    {
        var nameMsg = packet.GetDataStruct<TS_CS_CHECK_CHARACTER_NAME>();

        if (string.IsNullOrEmpty(nameMsg.Name))
        {
            client.SendResult(packet.Id, (ushort)ResultCode.AccessDenied);

            _logger.Debug("Character Name Check Failed! Empty Name for ({accountName}) {clientTag} !!!", client.ConnectionInfo.AccountName, client.ClientTag);

            return;
        }

        if (!nameMsg.Name.IsValidName(4, 18))
        {
            client.SendResult(packet.Id, (ushort)ResultCode.InvalidText);

            _logger.Debug("Character Name Check Failed! Invalid Name ({name}) for ({accountName}) {clientTag} !!!", nameMsg.Name, client.ConnectionInfo.AccountName, client.ClientTag);

            return;
        }
        
        if (_bannedWordsRepository.ContainsBannedWord(nameMsg.Name))
        {
            client.SendResult(packet.Id, (ushort)ResultCode.InvalidText);

            _logger.Debug("Character Name Check Failed! Name ({name}) contains banned word! for ({accountName}) {clientTag} !!!", nameMsg.Name, client.ConnectionInfo.AccountName, client.ClientTag);

            return;
        }

        if (_characterService.CharacterExists(nameMsg.Name)) 
        {
            client.SendResult(packet.Id, (ushort)ResultCode.AlreadyExist);

            _logger.Debug("Character Name Check Failed! Name ({name}) already exists! for ({accountName}) {clientTag} !!!", nameMsg.Name, client.ConnectionInfo.AccountName, client.ClientTag);

            return;
        }

        _logger.Debug("Character Name Check Passed! for ({accountName}) {clientTag}", client.ConnectionInfo.AccountName, client.ClientTag);

        client.SendResult(packet.Id, (ushort)ResultCode.Success);
    }


    private void OnAccountWithAuth(GameClient client, IPacket packet)
    {
        _logger.Debug("{clientTag} verifying with Auth Server", client.ClientTag);

        var msg = packet.GetDataStruct<TS_CS_ACCOUNT_WITH_AUTH>();
        var loginInfo = new Packet<TS_GA_CLIENT_LOGIN>((ushort)AuthPackets.TS_GA_CLIENT_LOGIN,
            new TS_GA_CLIENT_LOGIN(msg.Account, msg.OneTimePassword));

        if (_networkService.AuthorizedGameClients.Count > _networkService.NetworkOptions.MaxConnections)
        {
            client.SendResult(packet.Id, (ushort)ResultCode.LimitMax);
        }

        if (string.IsNullOrEmpty(client.ConnectionInfo.AccountName))
        {
            if (_networkService.UnauthorizedGameClients.ContainsKey(msg.Account))
            {
                client.SendResult(packet.Id, (ushort)ResultCode.AccessDenied);
                return;
            }

            _networkService.UnauthorizedGameClients.Add(msg.Account, client);
        }
    
        _networkService.AuthClient.SendMessage(loginInfo);
    }
}