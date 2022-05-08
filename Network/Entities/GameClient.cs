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

namespace Navislamia.Network.Objects
{
    using static Navislamia.Network.Packets.Extensions;

    public class GameClient : Client
    {
        protected byte[] messageBuffer;

        protected IConfigurationService configSVC;
        protected IGameActionService gameActionsSVC;

        XRC4Cipher recvCipher = new XRC4Cipher();
        XRC4Cipher sendCipher = new XRC4Cipher();

        protected ConcurrentQueue<ISerializablePacket> recvMsgQueue = new ConcurrentQueue<ISerializablePacket>();
        protected BlockingCollection<ISerializablePacket> recvMsgCollection;

        protected ConcurrentQueue<ISerializablePacket> sendMsgQueue = new ConcurrentQueue<ISerializablePacket>();
        protected BlockingCollection<ISerializablePacket> sendMsgCollection;

        public GameClient(Socket socket, int length, IConfigurationService configurationService, INotificationService notificationService, INetworkService networkService, IGameActionService actionService) : base(socket, length, configurationService, notificationService, networkService) 
        {
            messageBuffer = new byte[BufferLen];
            Data = new byte[BufferLen];

            configSVC = configurationService;
            gameActionsSVC = actionService;

            var key = configSVC.Get<string>("cipher.key", "Network");

            recvCipher.SetKey(key);
            sendCipher.SetKey(key);

            recvMsgCollection = new BlockingCollection<ISerializablePacket>(recvMsgQueue);
            sendMsgCollection = new BlockingCollection<ISerializablePacket>(sendMsgQueue);

            List<Task> tasks = new List<Task>();

            tasks.Add(Task.Run(() =>
            {
                ISerializablePacket msg;

                while (true)
                    if (sendMsgCollection.IsAddingCompleted && !sendMsgCollection.IsCompleted)
                        while (sendMsgCollection.TryTake(out msg))
                            Send(msg);
            }));

            tasks.Add(Task.Run(() =>
            {
                ISerializablePacket msg;

                while (true)
                {
                    if (recvMsgCollection.IsAddingCompleted && !recvMsgCollection.IsCompleted)
                        while (recvMsgCollection.TryTake(out msg))
                            gameActionsSVC.Execute(this, msg);
                }
            }));

            Task.WhenAll(tasks);
        }

        public override void Send(ISerializablePacket msg, bool beginReceive = true)
        {
            base.Send(msg, beginReceive);
        }

        public void SendResult(ClientPackets id, ushort result, int value = 0)
        {
            sendMsgCollection.Add(new TS_SC_RESULT((ushort)id, result, value));

            sendMsgCollection.CompleteAdding();
        }

        public override void Receive()
        {
            if (!Socket.Connected)
                return;

            Socket.BeginReceive(messageBuffer, 0, messageBuffer.Length, SocketFlags.None, ReceiveCallback, this);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            if (ConfigurationService.Get<bool>("debug", "Runtime", false))
                NotificationService.WriteDebug("Receiving data from the game client...");

            GameClient client = (GameClient)ar.AsyncState;

            // TODO: make sure the connection hasn't be unexpectedly closed

            int availableBytes = client.Socket.EndReceive(ar);

            if (DebugPackets)
                NotificationService.WriteDebug($"{availableBytes} bytes received from the game client!");

            if (client.Data.Length < DataOffset + availableBytes)
            {

            }

            // TODO: decode what data we have received
            byte[] decodedBuffer = new byte[availableBytes];

            recvCipher.Decode(messageBuffer, decodedBuffer, availableBytes);

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
                    recvMsgCollection.Add(msg);

                    if (DebugPackets)
                        NotificationService.WriteString(((Packet)msg).DumpToString());
                }

                // move the remaining bytes to the front of client data
                Buffer.BlockCopy(client.Data, msgLength, client.Data, 0, msgLength);

                // Reduce the data offset by the amount of bytes we have dropped from client data
                DataOffset -= msgLength;
            }

            recvMsgCollection.CompleteAdding();

            Receive();
        }
    }
}
