using Configuration;
using Navislamia.Network.Packets;
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
        XRC4Cipher recvCipher = new XRC4Cipher();
        XRC4Cipher sendCipher = new XRC4Cipher();

        protected byte[] encryptedData;
        protected IGameActionService actionSVC;

        public GameClient(Socket socket, int length, IConfigurationService configurationService, INotificationService notificationService, INetworkService networkService, IGameActionService actionService) : base(socket, length, configurationService, notificationService, networkService) 
        {
            encryptedData = new byte[BufferLen];

            actionSVC = actionService;
        }

        public override void Send(Packet msg, bool beginReceive = true)
        {
            base.Send(msg, beginReceive);
        }

        public override void Receive()
        {
            if (!Socket.Connected)
                return;

            Socket.BeginReceive(encryptedData, 0, encryptedData.Length, SocketFlags.None, ReceiveCallback, this);
        }

        private void ReceiveCallback(IAsyncResult ar) 
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

            client.Data = new byte[encryptedData.Length];

            // TODO: decode the data
            recvCipher.Decode(encryptedData, client.Data, encryptedData.Length);

            // TODO: deserialize the gameclient data
            var header = Header.GetPacketHeader(client.Data);

            
        }
    }
}
