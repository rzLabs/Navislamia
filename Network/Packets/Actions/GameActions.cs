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
using Navislamia.Network.Objects;
using Navislamia.Network.Packets.Actions.Interfaces;

namespace Navislamia.Network.Packets.Actions
{
    public class GameActions : IGameActionService
    {
        IConfigurationService configSVC;
        INotificationService notificationSVC;

        Dictionary<ushort, Func<GameClient, ISerializablePacket, int>> actions = new Dictionary<ushort, Func<GameClient, ISerializablePacket, int>>();

        public GameActions(IConfigurationService configService, INotificationService notificationService)
        {
            configSVC = configService;
            notificationSVC = notificationService;

            actions.Add((ushort)ClientPackets.TM_CS_VERSION, OnVersion);
            actions.Add((ushort)ClientPackets.TM_CS_ACCOUNT_WITH_AUTH, OnAccountWithAuth);
        }

        public int Execute(GameClient client, ISerializablePacket msg)
        {
            if (!actions.ContainsKey(msg.ID))
                return 1;

            return actions[msg.ID]?.Invoke(client, msg) ?? 2;
        }

        private int OnVersion(GameClient client, ISerializablePacket arg)
        {
            return 0;
        }

        public int OnAccountWithAuth(GameClient client, ISerializablePacket msg)
        {
            var _msg = msg as TM_CS_ACCOUNT_WITH_AUTH;

            // TODO: implement sending result

            return 0;
        }
    }
}
