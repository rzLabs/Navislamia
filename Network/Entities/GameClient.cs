using Configuration;
using Navislamia.Network.Enums;
using Navislamia.Network.Extensions;
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

namespace Navislamia.Network.Objects
{
    using static Navislamia.Network.Packets.Extensions;
    using static Navislamia.Network.Extensions.GameClientExtensions;

    public class GameClient : Client
    {
        protected byte[] messageBuffer;

        protected IConfigurationService configSVC;
        protected IGameActionService gameActionsSVC;

        protected ConcurrentQueue<ISerializablePacket> msgQueue = new ConcurrentQueue<ISerializablePacket>();

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

            Socket.BeginReceive(messageBuffer, 0, messageBuffer.Length, SocketFlags.None, ReceiveCallback, this);
        }

        private void ReceiveCallback(IAsyncResult ar)  // TODO: implement message queue
        {
            if (ConfigurationService.Get<bool>("debug", "Runtime", false))
                NotificationService.WriteDebug("Receiving data from the game client...");

            GameClient client = (GameClient)ar.AsyncState;

            int availableBytes = client.Socket.EndReceive(ar);

            if (DebugPackets)
                NotificationService.WriteDebug($"{availableBytes} bytes received from the game client!");

            if (client.Data.Length < DataOffset + availableBytes)
            {

            }

            // TODO: decode what data we have received
            byte[] decodedBuffer = new byte[availableBytes];

            RecvCipher.Decode(messageBuffer, decodedBuffer, availableBytes);

            // move the decoded data into the client.Data storage at front
            Buffer.BlockCopy(decodedBuffer, 0, client.Data, DataOffset, availableBytes);

            // increase the offset by the amount of bytes we are writing to the client data
            DataOffset += availableBytes;

            // Process data in the actual client
            while (DataOffset >= 4)
            {
                // Get a pointer to the client data
                Span<byte> data = client.Data;

                // Read the messages length
                int msgLength = BitConverter.ToInt32(data.Slice(0, 4));

                if (msgLength > DataOffset || msgLength == 0)
                    break;

                // buffer the message
                byte[] msgBuffer = ((Span<byte>)client.Data).Slice(0, msgLength).ToArray();

                PacketHeader header = Header.GetPacketHeader(msgBuffer);
                ISerializablePacket msg = null;

                if (!Enum.IsDefined(typeof(ClientPackets), (int)header.ID))
                {
                    // TODO: packet is not implemented
                }

                switch (header.ID)
                {
                    case (int)ClientPackets.TM_NONE:
                        break;

                    case (int)ClientPackets.TM_CS_VERSION:
                        msg = new TM_CS_VERSION(msgBuffer);
                        break;

                    case (int)ClientPackets.TM_CS_ACCOUNT_WITH_AUTH:
                        msg = new TM_CS_ACCOUNT_WITH_AUTH(msgBuffer);
                        break;
                }

                // add message to the queue
                if (msg is not null)
                {
                    msgQueue.Enqueue(msg);

                    if (DebugPackets)
                        NotificationService.WriteString(((Packet)msg).DumpToString());
                }

                // move the remaining bytes to the front of client data
                Buffer.BlockCopy(client.Data, msgLength, client.Data, 0, msgLength);

                // Reduce the data offset by the amount of bytes we have dropped from client data
                DataOffset -= msgLength;
            }

            Task.Run(() => processMsgQueue());

            Receive();
        }

        private void processMsgQueue()
        {
            foreach (var queuedMsg in msgQueue)
            {
                ISerializablePacket msg = null;

                if (!msgQueue.TryDequeue(out msg))
                {
                    // TODO: something happened
                    continue;
                }

                gameActionsSVC.Execute(this, msg);
            }
        }
    }
}
