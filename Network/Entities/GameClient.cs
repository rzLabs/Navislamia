using Configuration;
using Navislamia.Network.Packets;
using Network;
using Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Objects
{
    using static Navislamia.Network.Packets.Actions;

    public class GameClient : Client
    {
        protected byte[] encryptedData;

        public GameClient(Socket socket, int length, IConfigurationService configurationService, INotificationService notificationService, INetworkService networkService) : base(socket, length, configurationService, notificationService, networkService) 
        {
            encryptedData = new byte[BufferLen];
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

            // TODO: deserialize the gameclient data
            var header = Header.GetPacketHeader(client.Data);

            // TODO: if header.ID == GAMEPACKET.ID
            // TODO: Construct proper packet type
            // TODO: msg.Actions.OnReceive()
        }
    }
}
