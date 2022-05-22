using Configuration;
using Navislamia.Network.Enums;
using Navislamia.Network.Packets;
using Navislamia.Network.Packets.Actions.Interfaces;
using Navislamia.Network.Packets.Game;
using Network;
using Network.Security;
using Notification;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Entities
{
    using static Navislamia.Network.Packets.Extensions;

    public class GameClient : Client
    {

        protected IGameActionService gameActionsSVC;

        public GameClient(Socket socket, int length, IConfigurationService configurationService, INotificationService notificationService, INetworkService networkService, IGameActionService actionService) : base(socket, length, configurationService, notificationService, networkService, null, null, actionService)
        {
            Data = new byte[BufferLen];

        }

        public void SendResult(ushort id, ushort result, int value = 0)
        {
            PendMessage(new TS_SC_RESULT(id, result, value));
        }
    }
}
