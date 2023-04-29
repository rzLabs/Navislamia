using Configuration;
using Navislamia.Network.Enums;
using Network;
using Navislamia.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Navislamia.Network.Interfaces;
using Navislamia.Network.Packets.Game;
using Navislamia.Network.Packets.Actions.Interfaces;
using Navislamia.Network.Entities;
using Navislamia.Network.Packets.Auth;

namespace Navislamia.Network.Packets.Actions
{
    public class GameActions : IGameActionService
    {
        IConfigurationService configSVC;
        INotificationService notificationSVC;
        INetworkService networkSVC;

        Dictionary<ushort, Func<IClient, ISerializablePacket, int>> actions = new Dictionary<ushort, Func<IClient, ISerializablePacket, int>>();

        public GameActions(IConfigurationService configService, INotificationService notificationService, INetworkService networkService)
        {
            configSVC = configService;
            notificationSVC = notificationService;
            networkSVC = networkService;

            actions.Add((ushort)ClientPackets.TM_CS_VERSION, OnVersion);
            actions.Add((ushort)ClientPackets.TS_CS_REPORT, onReport);
            actions.Add((ushort)ClientPackets.TS_CS_CHARACTER_LIST, onCharacterList);
            actions.Add((ushort)ClientPackets.TM_CS_ACCOUNT_WITH_AUTH, OnAccountWithAuth);
        }

        public int Execute(IClient client, ISerializablePacket msg)
        {
            if (!actions.ContainsKey(msg.ID))
                return 1;

            return actions[msg.ID]?.Invoke(client, msg) ?? 2;
        }

        private int OnVersion(IClient client, ISerializablePacket arg)
        {
            // TODO: properly implement this action

            return 0;
        }

        private int onReport(IClient arg1, ISerializablePacket arg2)
        {
            // TODO: implement me

            return 0;
        }

        private int onCharacterList(IClient arg1, ISerializablePacket arg2)
        {
            // TODO: implement me

            return 0;
        }

        public int OnAccountWithAuth(IClient client, ISerializablePacket msg)
        {
            var gameClient = client as GameClient;
            var _msg = msg as TM_CS_ACCOUNT_WITH_AUTH;
            var _loginInfo = new TS_GA_CLIENT_LOGIN(_msg.Account, _msg.OneTimePassword);

            var connMax = configSVC.Get<int>("io.max_connection", "network", 2000);

            if (networkSVC.PlayerCount > connMax)
            {
                gameClient.SendResult(msg.ID, (ushort)ResultCode.LimitMax);
                return 1;
            }

            if (string.IsNullOrEmpty(gameClient.Info.AccountName.String))
            {
                if (networkSVC.AuthAccounts.ContainsKey(_msg.Account.String))
                {
                    gameClient.SendResult(msg.ID, (ushort)ResultCode.AccessDenied);
                    return 1;
                }

                networkSVC.AuthAccounts.Add(_msg.Account.String, gameClient);
            }

            if (networkSVC.AuthClient.GetEntity().Connected)
                networkSVC.AuthClient.PendMessage(_loginInfo);

            return 0;
        }
    }
}
