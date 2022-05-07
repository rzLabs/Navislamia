using Configuration;
using Navislamia.Network.Enums;
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

    public class GameClient : Client
    {
        protected byte[] messageBuffer;

        protected IConfigurationService configSVC;
        protected IGameActionService gameActionsSVC;

        XRC4Cipher recvCipher = new XRC4Cipher();
        XRC4Cipher sendCipher = new XRC4Cipher();

        public GameClient(Socket socket, int length, IConfigurationService configurationService, INotificationService notificationService, INetworkService networkService, IGameActionService actionService) : base(socket, length, configurationService, notificationService, networkService) 
        {

            messageBuffer = new byte[BufferLen];

            configSVC = configurationService;
            gameActionsSVC = actionService;

            var key = configSVC.Get<string>("cipher.key", "Network");

            recvCipher.SetKey(key);
            sendCipher.SetKey(key);
        }

        public override void Send(Packet msg, bool beginReceive = true)
        {
            base.Send(msg, beginReceive);
        }

        public override void Receive()
        {
            if (!Socket.Connected)
                return;

            Socket.BeginReceive(messageBuffer, 0, messageBuffer.Length, SocketFlags.None, ReceiveCallback, this);
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

            // Client needs to be ready to accept data
            client.Data = new byte[messageBuffer.Length];

            // Decode the buffer to get header
            recvCipher.Decode(messageBuffer, client.Data, messageBuffer.Length, true);

            var header = Header.GetPacketHeader(client.Data);

            // Actually decode the stream from t
            recvCipher.Decode(messageBuffer, client.Data, (int)header.Length);

            Buffer.BlockCopy(client.Data, 27, client.Data, 0, (int)header.Length);

            if (header.ID == (ushort)ClientPackets.TM_CS_VERSION)
            {
                var msg = new TM_CS_VERSION(client.Data);

                if (!msg.IsValid())
                {
                    NotificationService.WriteMarkup($"[bold red]{msg.GetType().Name} bears an invalid checksum![/]");
                    return;
                }

                gameActionsSVC.Execute(msg);
            }
        }
    }
}
