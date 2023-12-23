using System;
using System.Collections.Generic;
using Configuration;
using Navislamia.Game.Network.Entities;
using Navislamia.Network.Packets;
using Navislamia.Game.Services;
using System.Linq;

using Navislamia.Game.Network.Interfaces;
using Navislamia.Game.Models.Arcadia.Enums;

using Serilog;

namespace Navislamia.Game.Network.Packets
{
    public class GameActionService
    {
        private readonly NetworkOptions _networkOptions;
        IClientService _clientService;
        ILogger _logger = Log.ForContext<GameActionService>();
        ICharacterService _characterService;

        Dictionary<ushort, Func<GameClient, IPacket, int>> actions = new();

        public GameActionService(IClientService clientService, NetworkOptions networkOptions, ICharacterService characterService)
        {
            _clientService = clientService;
            _networkOptions = networkOptions;
            _characterService = characterService;

            actions.Add((ushort)GamePackets.TM_CS_VERSION, OnVersion);
            actions.Add((ushort)GamePackets.TS_CS_REPORT, OnReport);
            actions.Add((ushort)GamePackets.TS_CS_CHARACTER_LIST, OnCharacterList);
            actions.Add((ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH, OnAccountWithAuth);
        }

        public void Execute(GameClient client, IPacket msg)
        {
            if (!actions.ContainsKey(msg.ID))
                return;

            actions[msg.ID]?.Invoke(client, msg);
        }

        public void RemoveGameClient(string account, GameClient client)
        {
            _clientService.RemoveGameClient(account, client);
        }
        
        private int OnVersion(GameClient client, IPacket arg)
        {
            // TODO: properly implement this action

            return 0;
        }

        private int OnReport(GameClient arg1, IPacket arg2)
        {
            // TODO: implement me

            return 0;
        }

        private int OnCharacterList(GameClient client, IPacket msg)
        {
            var _msg = msg.GetDataStruct<TS_CS_CHARACTER_LIST>();

            var _characters = _characterService.GetCharactersByAccountName(_msg.Account, true);

            List<LobbyCharacterInfoEntity> _lobbyCharacters = new List<LobbyCharacterInfoEntity>();

            foreach (var _character in _characters)
            {
                var _characterLobbyInfo = new LobbyCharacterInfoEntity();

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
                _characterLobbyInfo.DeleteTime = _character.DeletedOn?.ToString("yyyy/MM/dd") ?? "9999/12/01";

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

        private int OnAccountWithAuth(GameClient client, IPacket msg)
        {
            var _msg = msg.GetDataStruct<TM_CS_ACCOUNT_WITH_AUTH>();
            var _loginInfo = new Packet<TS_GA_CLIENT_LOGIN>((ushort)AuthPackets.TS_GA_CLIENT_LOGIN, new(_msg.Account, _msg.OneTimePassword));

            var connMax = _networkOptions.MaxConnections;

            if (_clientService.ClientCount > connMax)
            {
                client.SendResult(msg.ID, (ushort)ResultCode.LimitMax);
                return 1;
            }

            if (string.IsNullOrEmpty(client.Info.AccountName))
            {
                if (_clientService.UnauthorizedGameClients.ContainsKey(_msg.Account))
                {
                    client.SendResult(msg.ID, (ushort)ResultCode.AccessDenied);
                    return 1;
                }

                _clientService.UnauthorizedGameClients.Add(_msg.Account, client);
            }

            _clientService.AuthClient.SendMessage(_loginInfo);

            return 0;
        }
    }
}
