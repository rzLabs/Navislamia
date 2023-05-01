using Configuration;
using Navislamia.Network.Enums;
using Network;
using Navislamia.Notification;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Navislamia.Network.Packets.Game;
using Navislamia.Network.Entities;
using Navislamia.Network.Packets.Auth;

namespace Navislamia.Network.Packets.Actions
{
    public class GameActions
    {
        private readonly NetworkOptions _networkOptions;
        INotificationService notificationSVC;
        INetworkService networkSVC;

        Dictionary<ushort, Func<ClientService<GameClientEntity>, ISerializablePacket, int>> actions = new();

        public GameActions(IOptions<NetworkOptions> networkOptions, INotificationService notificationService, INetworkService networkService)
        {
            _networkOptions = networkOptions.Value;
            notificationSVC = notificationService;
            networkSVC = networkService;

            actions.Add((ushort)ClientPackets.TM_CS_VERSION, OnVersion);
            actions.Add((ushort)ClientPackets.TS_CS_REPORT, OnReport);
            actions.Add((ushort)ClientPackets.TS_CS_CHARACTER_LIST, OnCharacterList);
            actions.Add((ushort)ClientPackets.TM_CS_ACCOUNT_WITH_AUTH, OnAccountWithAuth);
        }

        public int Execute(ClientService<GameClientEntity> client, ISerializablePacket msg)
        {
            if (!actions.ContainsKey(msg.ID))
                return 1;

            return actions[msg.ID]?.Invoke(client, msg) ?? 2;
        }

        private int OnVersion(ClientService<GameClientEntity> client, ISerializablePacket arg)
        {
            // TODO: properly implement this action

            return 0;
        }

        private int OnReport(ClientService<GameClientEntity> arg1, ISerializablePacket arg2)
        {
            // TODO: implement me

            return 0;
        }

        private int OnCharacterList(ClientService<GameClientEntity> arg1, ISerializablePacket arg2)
        {
            // TODO: implement me

            return 0;
        }

        private int OnAccountWithAuth(ClientService<GameClientEntity> client, ISerializablePacket msg)
        {
            var _msg = msg as TM_CS_ACCOUNT_WITH_AUTH;
            var _loginInfo = new TS_GA_CLIENT_LOGIN(_msg.Account, _msg.OneTimePassword);

            var connMax = _networkOptions.MaxConnections;

            if (networkSVC.PlayerCount > connMax)
            {
                client.SendResult(msg.ID, (ushort)ResultCode.LimitMax);
                return 1;
            }

            if (string.IsNullOrEmpty(client.GetEntity().Info.AccountName.String))
            {
                if (networkSVC.UnauthorizedGameClients.ContainsKey(_msg.Account.String))
                {
                    client.SendResult(msg.ID, (ushort)ResultCode.AccessDenied);
                    return 1;
                }

                networkSVC.UnauthorizedGameClients.Add(_msg.Account.String, client);
            }

            if (networkSVC.AuthClient.GetEntity().Connected)
                networkSVC.AuthClient.PendMessage(_loginInfo);

            return 0;
        }
    }
}
