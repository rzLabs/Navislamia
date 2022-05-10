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
        protected byte[] messageBuffer;

        protected IGameActionService gameActionsSVC;

        public GameClient(Socket socket, int length, IConfigurationService configurationService, INotificationService notificationService, INetworkService networkService, IGameActionService actionService) : base(socket, length, configurationService, notificationService, networkService, null, null, actionService)
        {
            messageBuffer = new byte[BufferLen];
            Data = new byte[BufferLen];

            gameActionsSVC = actionService;
        }

        public void SendResult(ushort id, ushort result, int value = 0)
        {
            PendMessage(new TS_SC_RESULT((ushort)id, result, value));
        }

        public override void PendMessage(ISerializablePacket msg) => base.PendMessage(msg);

        public override void Send(ISerializablePacket msg)
        {
            if (DebugPackets)
            {
                NotificationService.WriteDebug($"Sending {msg.Length} bytes of data to Game client @ {IP}");
                NotificationService.WriteMarkup(((Packet)msg).DumpToString());
            }

            byte[] encodedBuffer = new byte[msg.Length];

            SendCipher.Encode(msg.Data, encodedBuffer, (int)msg.Length);

            Socket.BeginSend(encodedBuffer, 0, encodedBuffer.Length, SocketFlags.None, SendCallback, this);

            Listen();
        }

        private void SendCallback(IAsyncResult ar)
        {
            GameClient client = (GameClient)ar.AsyncState;

            // TODO: do something with this information
            int bytesSent = client.Socket.EndSend(ar);

            Listen();
        }

        public override void Listen()
        {
            if (!Socket.Connected)
                return;

            Socket.BeginReceive(messageBuffer, 0, messageBuffer.Length, SocketFlags.None, ListenCallback, this);
        }

        private void ListenCallback(IAsyncResult ar)
        {
            GameClient client = (GameClient)ar.AsyncState;

            if (!Socket.Connected)
            {
                NotificationService.WriteError("Read attempted for closed game client!");
                return;
            }

            int availableBytes = client.Socket.EndReceive(ar);

            if (availableBytes == 0)
                goto Listen;

            if (DebugPackets)
            {
                NotificationService.WriteDebug($"Receiving {availableBytes} bytes from a game client @ {client.IP}");
            }

            if (client.Data.Length < DataOffset + availableBytes)
                goto Listen;

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
                    NotificationService.WriteWarning($"Undefined packet {header.ID} (Checksum: {header.Checksum}, Length: {header.Length}) received from game client @ {client.IP}");

                    goto Listen;
                }

                switch (header.ID)
                {
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
                    RecvMsgCollection.Add(msg);

                    if (DebugPackets)
                        NotificationService.WriteMarkup(((Packet)msg).DumpToString());
                }

                client.Data = data.Slice(msgLength, data.Length - msgLength).ToArray();

                // move the remaining bytes to the front of client data
                //Buffer.BlockCopy(client.Data, msgLength, client.Data, 0, msgLength);

                // Reduce the data offset by the amount of bytes we have dropped from client data
                DataOffset -= msgLength;
            }

            RecvMsgCollection.CompleteAdding();

        Listen:

            Listen();
        }
    }
}
