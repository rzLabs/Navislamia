using Configuration;
using Navislamia.Network.Enums;
using Network;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Navislamia.Network.Packets.Game;
using Navislamia.Network.Packets.Actions.Interfaces;
using Navislamia.Network.Entities;
using Navislamia.Network.Packets.Auth;
using Navislamia.Notification;

namespace Navislamia.Network.Packets.Actions
{
    public class GameActions : IGameActionService
    {
        INotificationService notificationSVC;
        INetworkService networkSVC;
        private readonly NetworkOptions _networkOptions;

        private Dictionary<ushort, Func<Client, ISerializablePacket, int>> _actions = new();

        public GameActions(IOptions<NetworkOptions> networkOptions, INotificationService notificationService, INetworkService networkService)
        {
            notificationSVC = notificationService;
            networkSVC = networkService;
            _networkOptions = networkOptions.Value;

            _actions.Add((ushort)ClientPackets.TM_CS_VERSION, OnVersion);
            _actions.Add((ushort)ClientPackets.TS_CS_REPORT, onReport);
            _actions.Add((ushort)ClientPackets.TS_CS_CHARACTER_LIST, onCharacterList);
            _actions.Add((ushort)ClientPackets.TM_CS_ACCOUNT_WITH_AUTH, OnAccountWithAuth);
        }

        public int Execute(Client client, ISerializablePacket msg)
        {
            if (!_actions.ContainsKey(msg.ID))
                return 1;

            return _actions[msg.ID]?.Invoke(client, msg) ?? 2;
        }

        private int OnVersion(Client client, ISerializablePacket arg)
        {
            // TODO: properly implement this action

            return 0;
        }

        private int onReport(Client arg1, ISerializablePacket arg2)
        {
            // TODO: implement me

            return 0;
        }

        private int onCharacterList(Client arg1, ISerializablePacket arg2)
        {
            // TODO: implement me

            return 0;
        }

        public int OnAccountWithAuth(Client client, ISerializablePacket msg)
        {
            var _client = client as GameClient;
            var _msg = msg as TM_CS_ACCOUNT_WITH_AUTH;
            var _loginInfo = new TS_GA_CLIENT_LOGIN(_msg.Account, _msg.OneTimePassword);

            var connMax = _networkOptions.MaxConnections;

            if (networkSVC.PlayerCount > connMax)
            {
                _client.SendResult(msg.ID, (ushort)ResultCode.LimitMax);
                return 1;
            }

            if (string.IsNullOrEmpty(_client.ClientInfo.AccountName.String))
            {
                if (networkSVC.AuthAccounts.ContainsKey(_msg.Account.String))
                {
                    _client.SendResult(msg.ID, (ushort)ResultCode.AccessDenied);
                    return 1;
                }

                networkSVC.AuthAccounts.Add(_msg.Account.String, _client);
            }

            if (networkSVC.AuthClient?.Connected ?? false)
                networkSVC.AuthClient.PendMessage(_loginInfo);

            return 0;
        }
    }
}
