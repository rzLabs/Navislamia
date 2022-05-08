using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using System.Net;
using System.Net.Sockets;


using Navislamia.Network.Enums;
using Navislamia.Network.Packets;
using Navislamia.Network.Packets.Auth;
using Navislamia.Network.Packets.Actions;
using static Navislamia.Network.Packets.Extensions;

using Notification;
using Network;
using Configuration;
using Navislamia.Network.Packets.Actions.Interfaces;

namespace Navislamia.Network.Entities
{
    public class AuthClient : Client
    {
        protected byte[] messageBuffer;

        IAuthActionService authActions;

        public AuthClient(Socket socket, int length, IConfigurationService configurationService, INotificationService notificationService, INetworkService networkService, IAuthActionService actions) : base(socket, length, configurationService, notificationService, networkService, actions, null, null) 
        {
            messageBuffer = new byte[BufferLen];
            Data = new byte[BufferLen];

            authActions = actions;
        }

        public override void PendMessage(ISerializablePacket msg) => base.PendMessage(msg);

        public override void Send(ISerializablePacket msg)
        {
            if (DebugPackets)
            {
                NotificationService.WriteDebug($"Sending {msg.Length} bytes of data to Auth Server!");
                NotificationService.WriteString(((Packet)msg).DumpToString());
            }

            Socket.BeginSend(msg.Data, 0, msg.Data.Length, SocketFlags.None, SendCallback, this);
        }     

        private void SendCallback(IAsyncResult ar)
        {
            AuthClient client = (AuthClient)ar.AsyncState;

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
            AuthClient client = (AuthClient)ar.AsyncState;

            if (!Socket.Connected)
            {
                NotificationService.WriteError("Read attempted for closed Auth client!");
                return;
            }

            int availableBytes = client.Socket.EndReceive(ar);

            // if there is no data, just start listening again
            if (availableBytes == 0)
                goto Listen;

            if (DebugPackets)
                NotificationService.WriteDebug($"{availableBytes} bytes received from the Auth Server!");

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

                if (!Enum.IsDefined(typeof(AuthPackets), (int)header.ID))
                {
                    // TODO: packet is not implemented

                    goto Listen;
                }

                switch (header.ID)
                {
                    case (int)AuthPackets.TS_AG_LOGIN_RESULT:
                        msg = new TS_AG_LOGIN_RESULT(msgBuffer);
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
