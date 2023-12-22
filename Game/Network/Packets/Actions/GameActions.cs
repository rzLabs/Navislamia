using Navislamia.Network.Enums;
using Navislamia.Notification;
using System;
using System.Collections.Generic;
using Configuration;
using Navislamia.Game.Network;
using Navislamia.Game.Network.Entities;
using Navislamia.Network.Packets.Game;
using Navislamia.Network.Packets.Auth;
using Navislamia.Game.Services;
using System.Linq;
using Navislamia.Game.Models.Arcadia.Enums;

namespace Navislamia.Network.Packets.Actions
{
    public class GameActions
    {
        private readonly NetworkOptions _networkOptions;
        INotificationModule _notificationModule;
        INetworkModule _networkModule;
        ICharacterService _characterService;

        Dictionary<ushort, Func<ClientService<GameClientEntity>, IPacket, int>> actions = new();

        public GameActions(INotificationModule notificationModule, INetworkModule networkModule, NetworkOptions networkOptions, ICharacterService characterService)
        {
            _networkOptions = networkOptions;
            _notificationModule = notificationModule;
            _networkModule = networkModule;
            _characterService = characterService;

            actions.Add((ushort)GamePackets.TM_CS_VERSION, OnVersion);
            actions.Add((ushort)GamePackets.TS_CS_REPORT, OnReport);
            actions.Add((ushort)GamePackets.TS_CS_CHARACTER_LIST, OnCharacterList);
            actions.Add((ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH, OnAccountWithAuth);
        }

        public int Execute(ClientService<GameClientEntity> client, IPacket msg)
        {
            if (!actions.ContainsKey(msg.ID))
                return 1;

            return actions[msg.ID]?.Invoke(client, msg) ?? 2;
        }

        private int OnVersion(ClientService<GameClientEntity> client, IPacket arg)
        {
            // TODO: properly implement this action

            return 0;
        }

        private int OnReport(ClientService<GameClientEntity> arg1, IPacket arg2)
        {
            // TODO: implement me

            return 0;
        }

        private int OnCharacterList(ClientService<GameClientEntity> client, IPacket msg)
        {
            var _msg = msg.GetDataStruct<TS_CS_CHARACTER_LIST>();

            var _characters = _characterService.GetCharactersByAccountName(_msg.Account, true);

            List<LobbyCharacterInfo> _lobbyCharacters = new List<LobbyCharacterInfo>();

            foreach (var _character in _characters)
            {
                var _characterLobbyInfo = new LobbyCharacterInfo();

                _characterLobbyInfo.Level = _character.Lv;
                _characterLobbyInfo.Job = (int)_character.CurrentJob;
                _characterLobbyInfo.JobLevel = _character.Jlv;
                _characterLobbyInfo.ExpPercentage = 0; // TODO: needs to be done by getting values from LevelResourceRepository
                _characterLobbyInfo.HP = _character.Hp;
                _characterLobbyInfo.MP = _character.Mp;
                _characterLobbyInfo.Permission = _character.Permission;
                _characterLobbyInfo.IsBanned = 0;
                _characterLobbyInfo.Name = _character.CharacterName;
                _characterLobbyInfo.SkinColor = (uint)_character.SkinColor;
                _characterLobbyInfo.CreateTime = _character.CreatedOn.ToString("yyyy/MM/dd");
                _characterLobbyInfo.DeleteTime = _character.DeletedOn?.ToString("yyyy/MM/dd");

                foreach (var _item in  _character.Items.Where(i => i.WearInfo != ItemWearType.None))
                {
                    _characterLobbyInfo.WearInfo[(int)_item.WearInfo] = _item.ItemResourceId;
                    _characterLobbyInfo.WearItemEnhanceInfo[(int)_item.WearInfo] = _item.Enhance;
                    _characterLobbyInfo.WearItemLevelInfo[(int)_item.WearInfo] = _item.Level;
                    _characterLobbyInfo.WearItemElementalType[(int)_item.WearInfo] = (char)_item.ElementalEffectType;
                }

                _characterLobbyInfo.Sex = _character.Sex;
                _characterLobbyInfo.Race = _character.Race;

                _characterLobbyInfo.ModelId = _character.Models;
                _characterLobbyInfo.HairColorIndex = _character.HairColorIndex;
                _characterLobbyInfo.HairColorRGB = (uint)_character.HairColorRgb;
                _characterLobbyInfo.HideEquipFlag = (uint)_character.HideEquipFlag;
                _characterLobbyInfo.TextureID = _character.TextureId;

                _lobbyCharacters.Add(_characterLobbyInfo);
            }

            client.SendCharacterList(_lobbyCharacters);

            return 0;
        }

        private int OnAccountWithAuth(ClientService<GameClientEntity> client, IPacket msg)
        {
            var _msg = msg.GetDataStruct<TM_CS_ACCOUNT_WITH_AUTH>();
            var _loginInfo = new Packet<TS_GA_CLIENT_LOGIN>((ushort)AuthPackets.TS_GA_CLIENT_LOGIN, new(_msg.Account, _msg.OneTimePassword));

            var connMax = _networkOptions.MaxConnections;

            if (_networkModule.GetPlayerCount() > connMax)
            {
                client.SendResult(msg.ID, (ushort)ResultCode.LimitMax);
                return 1;
            }

            if (string.IsNullOrEmpty(client.GetEntity().Info.AccountName))
            {
                if (_networkModule.UnauthorizedGameClients.ContainsKey(_msg.Account))
                {
                    client.SendResult(msg.ID, (ushort)ResultCode.AccessDenied);
                    return 1;
                }

                _networkModule.UnauthorizedGameClients.Add(_msg.Account, client);
            }

            if (_networkModule.GetAuthClient().GetEntity().Connection.Connected)
                _networkModule.GetAuthClient().SendMessage(_loginInfo);

            return 0;
        }
    }
}
