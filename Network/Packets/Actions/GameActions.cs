using Configuration;
using Navislamia.Network.Enums;
using Network;
using Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        Dictionary<ushort, Func<Client, ISerializablePacket, int>> actions = new Dictionary<ushort, Func<Client, ISerializablePacket, int>>();

        public GameActions(IConfigurationService configService, INotificationService notificationService, INetworkService networkService)
        {
            configSVC = configService;
            notificationSVC = notificationService;
            networkSVC = networkService;

            actions.Add((ushort)ClientPackets.TM_CS_VERSION, OnVersion);
            actions.Add((ushort)ClientPackets.TM_CS_ACCOUNT_WITH_AUTH, OnAccountWithAuth);
        }

        public int Execute(Client client, ISerializablePacket msg)
        {
            if (!actions.ContainsKey(msg.ID))
                return 1;

            return actions[msg.ID]?.Invoke(client, msg) ?? 2;
        }

        private int OnVersion(Client client, ISerializablePacket arg)
        {
            return 0;
        }

        public int OnAccountWithAuth(Client client, ISerializablePacket msg)
        {
            var _client = client as GameClient;
            var _msg = msg as TM_CS_ACCOUNT_WITH_AUTH;
            var _loginInfo = new TS_GA_CLIENT_LOGIN(_msg.Account, _msg.OneTimePassword);

            if (networkSVC.AuthClient?.Connected ?? false)
                networkSVC.AuthClient.PendMessage(_loginInfo);

            // Send test result denied
            _client.SendResult((ClientPackets)msg.ID, (ushort)ResultCode.AccessDenied);

            return 0;
        }
    }
}
