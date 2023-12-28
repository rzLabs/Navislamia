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

namespace Navislamia.Game.Network.Entities.Actions;

public class GameActions
{
    private readonly ILogger _logger = Log.ForContext<GameActions>();
    private readonly ICharacterService _characterService;
    private readonly NetworkService _networkService;

    private readonly Dictionary<ushort, Action<GameClient, IPacket>> _actions = new();
    public AuthClient AuthClient { get; set; }
    public List<GameClient> AuthorizedClients { get; set; }

    public GameActions(List<GameClient> authorizedClients, ICharacterService characterService, NetworkService networkService)
    {
        _characterService = characterService;
        _networkService = networkService;
        AuthorizedClients = authorizedClients;

        _actions.Add((ushort)GamePackets.TM_CS_VERSION, OnVersion);
        _actions.Add((ushort)GamePackets.TS_CS_REPORT, OnReport);
        _actions.Add((ushort)GamePackets.TS_CS_CHARACTER_LIST, OnCharacterList);
        _actions.Add((ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH, OnAccountWithAuth);
    }
    
    public void Execute(GameClient client, IPacket packet)
    {
        if (_actions.TryGetValue(packet.ID, out var action))
        {
            Task.Run(() => action?.Invoke(client, packet));
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
    
    private void OnAccountWithAuth(GameClient client, IPacket packet)
    {
        var message = packet.GetDataStruct<TM_CS_ACCOUNT_WITH_AUTH>();
        var loginInfo = new Packet<TS_GA_CLIENT_LOGIN>((ushort)AuthPackets.TS_GA_CLIENT_LOGIN,
            new TS_GA_CLIENT_LOGIN(message.Account, message.OneTimePassword));

        if (AuthorizedClients.Count > _networkService.Options.MaxConnections)
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
}