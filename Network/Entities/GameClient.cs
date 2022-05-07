using Configuration;
using Navislamia.Network.Enums;
using Navislamia.Network.Extensions;
using Navislamia.Network.Packets;
using Navislamia.Network.Packets.Game;
using Network;
using Network.Security;
using Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Objects
{
    using static Navislamia.Network.Packets.GameActions;
    using static Navislamia.Network.Extensions.GameClientExtensions;

    public class GameClient : Client
    {
        protected byte[] messageBuffer;

        protected IConfigurationService configSVC;
        protected IGameActionService gameActionsSVC;

        public GameClient(Socket socket, int length, IConfigurationService configurationService, INotificationService notificationService, INetworkService networkService, IGameActionService actionService) : base(socket, length, configurationService, notificationService, networkService) 
        {

            messageBuffer = new byte[BufferLen];
            Data = new byte[BufferLen];

            configSVC = configurationService;
            gameActionsSVC = actionService;

            var key = configSVC.Get<string>("cipher.key", "Network");

            RecvCipher.SetKey(key);
            SendCipher.SetKey(key);
        }

        public override void Send(Packet msg, bool beginReceive = true)
        {
            base.Send(msg, beginReceive);
        }

        public override void Receive()
        {
            if (!Socket.Connected)
                return;

            Socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, ReceiveCallback, this);
        }

        private void ReceiveCallback(IAsyncResult ar)  // TODO: implement message queue
        {
            if (ConfigurationService.Get<bool>("debug", "Runtime", false))
                NotificationService.WriteDebug("Receiving data from the game client...");

            GameClient client = (GameClient)ar.AsyncState;

            int readCnt = client.Socket.EndReceive(ar);

            if (readCnt <= 0)
            {
                NotificationService.WriteMarkup("[bold red]Failed to read data from the game client![/]");
                return;
            }

            if (DebugPackets)
                NotificationService.WriteDebug($"{readCnt} bytes received from the game client!");

            var header = client.PeekHeader();

            client.Read(out messageBuffer, (int)header.Length);

            if (header.ID == (ushort)ClientPackets.TM_NONE)
                return;

            if (!Enum.IsDefined(typeof(AuthPackets), (int)header.ID)) // Unlisted packet
            {
                NotificationService.WriteWarning($"Unlisted packet received! ID: {header.ID} Length: {header.Length} Checksum: {header.Checksum}");

                return;
            }

            ISerializablePacket msg = null;

            switch (header.ID)
            {
                case (ushort)ClientPackets.TM_CS_VERSION:
                    msg = new TM_CS_VERSION(messageBuffer);
                    break;

                case (ushort)ClientPackets.TM_CS_ACCOUNT_WITH_AUTH:
                    msg = new TM_CS_ACCOUNT_WITH_AUTH(messageBuffer);
                    break;
            }

            if (!msg.IsValid())
            {
                NotificationService.WriteMarkup($"[bold red]{msg.GetType().Name} bears an invalid checksum![/]");
                return;
            }

            gameActionsSVC.Execute(msg);
        }
    }
}
