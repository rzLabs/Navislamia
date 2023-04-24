using Configuration;
using Navislamia.Network.Packets.Actions.Interfaces;
using Navislamia.Network.Packets.Game;
using Network;
using System.Net.Sockets;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Notification;

namespace Navislamia.Network.Entities
{
    public class GameClient : Client
    {

        protected IGameActionService gameActionsSVC;
        private readonly NetworkOptions _networkOptions;
        private readonly LogOptions _logOptions;
        public GameClient(Socket socket, int length, INotificationService notificationService,
            INetworkService networkService, IGameActionService actionService, IOptions<NetworkOptions> networkOptions, 
            IOptions<LogOptions> logOptions) : base(socket, length, notificationService, networkService, null,
            null, actionService, networkOptions, logOptions)
        {
            Data = new byte[BufferLen];

        }

        public void SendResult(ushort id, ushort result, int value = 0)
        {
            PendMessage(new TS_SC_RESULT(id, result, value));
        }
    }
}
