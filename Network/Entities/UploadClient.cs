using Configuration;
using Navislamia.Network.Enums;
using Navislamia.Network.Packets;
using Navislamia.Network.Packets.Actions;
using Navislamia.Network.Packets.Actions.Interfaces;
using Navislamia.Network.Packets.Upload;
using Network;
using Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Entities
{
    public class UploadClient : Client
    {
        protected byte[] messageBuffer;

        public UploadClient(Socket socket, int length, IConfigurationService configurationService, INotificationService notificationService, INetworkService networkService, IUploadActionService uploadActionService) : base(socket, length, configurationService, notificationService, networkService, null, uploadActionService, null)
        {
            messageBuffer = new byte[BufferLen];

        }

        public override void Send(ISerializablePacket msg)
        {
            if (!Socket.Connected)
                return;

            Socket.Send(msg.Data);

            if (ConfigurationService.Get<bool>("packet.debug", "Logs", false))
            {
                NotificationService.WriteDebug($"[orange3]Sending {msg.GetType().Name} ({msg.Data.Length} bytes) to the Upload Server...[/]");
                NotificationService.WriteString(((Packet)msg).DumpToString());
            }

            Data = new byte[512];

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
            var client = (UploadClient)ar.AsyncState;

            if (!Socket.Connected)
            {
                NotificationService.WriteError("Read attempted for closed Upload client!");
                return;
            }

            int availableBytes = client.Socket.EndReceive(ar);

            // if there is no data, just start listening again
            if (availableBytes == 0)
                goto Listen;

            if (DebugPackets)
                NotificationService.WriteDebug($"{availableBytes} bytes received from the Upload Server!");

            // If client.Data length is below our current offset + read bytes
            if (client.Data.Length < DataOffset + availableBytes)
                goto Listen;

            // move the messageBuffer data into the client.Data storage at dataOffset
            Buffer.BlockCopy(messageBuffer, 0, client.Data, DataOffset, availableBytes);

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

                if (!Enum.IsDefined(typeof(UploadPackets), (int)header.ID))
                {
                    // TODO: packet is not implemented

                    goto Listen;
                }

                switch (header.ID)
                {
                    case (int)UploadPackets.TS_US_LOGIN_RESULT:
                        msg = new TS_US_LOGIN_RESULT(msgBuffer);
                        break;
                }

                // add message to the queue
                if (msg is not null)
                {
                    RecvMsgCollection.Add(msg);

                    if (DebugPackets)
                        NotificationService.WriteString(((Packet)msg).DumpToString());
                }

                // move the remaining bytes to the front of client data
                Buffer.BlockCopy(client.Data, msgLength, client.Data, 0, msgLength);

                // Reduce the data offset by the amount of bytes we have dropped from client data
                DataOffset -= msgLength;
            }

            RecvMsgCollection.CompleteAdding();

        Listen:

            Listen();
        }
    }
}
