using Configuration;
using Navislamia.Network.Enums;
using Navislamia.Network.Packets;
using Navislamia.Network.Packets.Actions.Interfaces;
using Navislamia.Network.Packets.Auth;
using Navislamia.Network.Packets.Game;
using Navislamia.Network.Packets.Upload;
using Network.Security;
using Navislamia.Notification;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Network.Interfaces;

namespace Navislamia.Network.Entities
{
    public class MessageQueue : IMessageQueue
    {
        bool debugPackets = false;

        bool sendProcessing = false;
        bool recvProcessing = false;

        XRC4Cipher RecvCipher = new XRC4Cipher();
        XRC4Cipher SendCipher = new XRC4Cipher();

        ConcurrentQueue<QueuedMessage> sendQueue= new ConcurrentQueue<QueuedMessage>();
        BlockingCollection<QueuedMessage> sendCollection;

        ConcurrentQueue<QueuedMessage> recvQueue = new ConcurrentQueue<QueuedMessage>();
        BlockingCollection<QueuedMessage> recvCollection;
        
        INotificationService notificationSVC;

        IAuthActionService authActionSVC;
        IGameActionService gameActionSVC;
        IUploadActionService uploadActionSVC;
        
        private readonly NetworkOptions _networkOptions;
        private readonly LogOptions _logOptions;

        public MessageQueue(INotificationService notificationService, IAuthActionService authActionService,
            IGameActionService gameActionService, IUploadActionService uploadActionService,
            IOptions<NetworkOptions> networkOptions, IOptions<LogOptions> logOptions)
        {
            _networkOptions = networkOptions.Value;
            _logOptions = logOptions.Value;
            
            debugPackets = _logOptions.PacketDebug;
            notificationSVC = notificationService;

            RecvCipher.SetKey(_networkOptions.CipherKey);
            SendCipher.SetKey(_networkOptions.CipherKey);

            sendCollection = new BlockingCollection<QueuedMessage>(sendQueue);
            recvCollection = new BlockingCollection<QueuedMessage>(recvQueue);

            authActionSVC = authActionService;
            uploadActionSVC = uploadActionService;
            gameActionSVC = gameActionService;

            Task.Run(() =>
            {
                while (true)
                {
                    if (sendCollection.IsAddingCompleted && !sendProcessing)
                        ProcessQueue(QueueType.Send);

                    if (sendCollection.IsCompleted)
                        sendCollection = new BlockingCollection<QueuedMessage>(sendQueue);

                    Thread.Sleep(100);
                }
            });

            Task.Run(() =>
            {
                while (true)
                {
                    if (recvCollection.IsAddingCompleted && !recvProcessing)
                        ProcessQueue(QueueType.Receive);

                    if (recvCollection.IsCompleted)
                        recvCollection = new BlockingCollection<QueuedMessage>(recvQueue);

                    Thread.Sleep(100);
                }
            });
        }

        public void Finalize(QueueType type)
        {
            switch (type)
            {
                case QueueType.Send:
                    sendCollection.CompleteAdding();
                    break;

                case QueueType.Receive:
                    recvCollection.CompleteAdding();
                    break;
            }
        }

        public void PendSend(IClient client, ISerializablePacket msg)
        {
            if (!sendCollection.TryAdd(new QueuedMessage(client, msg)))
            {
                notificationSVC.WriteError($"Failed to add msg to send queue! ID: {msg.ID}");

                return;
            }
        }

        public void PendReceive(IClient client, ISerializablePacket msg)
        {
            if (!recvCollection.TryAdd(new QueuedMessage(client, msg)))
            {
                notificationSVC.WriteError($"Failed to add msg to send queue! ID: {msg.ID}");

                return;
            }
        }

        public void ProcessClientData(IClient client, byte[] data, int count)
        {
            var clientEntity = client.GetEntity();

            string clientName = (client is GameClient) ? ((GameClient)client).Info.AccountName.String : clientEntity.IP;

            string clientType = "Auth";

            if (client is GameClient)
                clientType = "Game";
            else if (client is UploadClient)
                clientType = "Upload";

            if (client is GameClient)
            {
                byte[] buffer = new byte[count];

                RecvCipher.Decode(data, buffer, count);

                Buffer.BlockCopy(buffer, 0, clientEntity.Data, clientEntity.DataOffset, count);
            }
            else
                Buffer.BlockCopy(data, 0, clientEntity.Data, clientEntity.DataOffset, count);

            // increase the offset by the amount of bytes we wrote to the client data
            clientEntity.DataOffset += count;

            // Process and queue messages to be read from the data
            while (clientEntity.DataOffset >= 4)
            {
                // Get a pointer to the client data
                Span<byte> clientData = clientEntity.Data;

                // Read the message length
                int msgLength = BitConverter.ToInt32(clientData.Slice(0, 4));

                // If the message length is invalid ignore this message and advance the buffer by 4 bytes
                if (msgLength < 0 || msgLength > clientEntity.DataOffset)
                {
                    notificationSVC.WriteWarning($"Invalid message received from {clientType} client @ {clientEntity.IP}!!! Packet Length: {msgLength} @ DataOffset: {clientEntity.DataOffset}");
                    notificationSVC.WriteWarning(Utilities.StringExt.ByteArrayToString(((Span<byte>)data).Slice(0, count).ToArray()));

                    // if msgLength is below 0, set it to 4, if it above offset, set to 4
                    msgLength = Math.Max(4, Math.Min(4, Math.Min(msgLength, clientEntity.DataOffset)));
                }
                else // process and queue the message data
                {                
                    byte[] msgBuffer = clientData.Slice(0, msgLength).ToArray();

                    PacketHeader header = Header.GetPacketHeader(msgBuffer);
                    ISerializablePacket msg = null;

                    if (!Enum.IsDefined(typeof(ClientPackets), header.ID))
                        notificationSVC.WriteWarning($"Undefined packet {header.ID} (Checksum: {header.Checksum}, Length: {header.Length}) received from {clientType} client {clientEntity.IP} [{clientEntity.Port}]@{clientName}");

                    switch (header.ID)
                    {
                        #region Auth Packets

                        case (int)AuthPackets.TS_AG_LOGIN_RESULT:
                            msg = new TS_AG_LOGIN_RESULT(msgBuffer);
                            break;

                        case (int)AuthPackets.TS_AG_CLIENT_LOGIN:
                            msg = new TS_AG_CLIENT_LOGIN(msgBuffer);
                            break;

                        #endregion

                        #region Game Packets

                        case (int)ClientPackets.TM_NONE:
                            notificationSVC.WriteWarning($"TM_NONE of {header.Length} received from client {clientEntity.IP}:{clientEntity.Port}@{clientName}");
                            break;

                        case (int)ClientPackets.TM_CS_VERSION:
                            msg = new TM_CS_VERSION(msgBuffer);
                            break;

                        case (int)ClientPackets.TS_CS_CHARACTER_LIST:
                            msg = new TS_CS_CHARACTER_LIST(msgBuffer);
                            break;

                        case (int)ClientPackets.TM_CS_ACCOUNT_WITH_AUTH:
                            msg = new TM_CS_ACCOUNT_WITH_AUTH(msgBuffer);
                            break;

                        case (int)ClientPackets.TS_CS_REPORT:
                            msg = new TS_CS_REPORT(msgBuffer);
                            break;

                        #endregion

                        #region Upload Packets

                        case (int)UploadPackets.TS_US_LOGIN_RESULT:
                            msg = new TS_US_LOGIN_RESULT(msgBuffer);
                            break;

                        #endregion
                    }

                    // add message to the queue
                    if (msg is not null)
                    {
                        if (debugPackets)
                        {
                            string packetDmp = ((Packet)msg).DumpToString();

                            notificationSVC.WriteMarkup($"[bold orange3]Receiving message from {clientEntity.IP}:{clientEntity.Port}@{clientName}[/]\n\n{packetDmp}");
                        }

                        PendReceive(client, msg);
                    }
                }

                // move the remaining bytes to the front of client data
                Buffer.BlockCopy(clientEntity.Data, msgLength, clientEntity.Data, 0, clientEntity.Data.Length - msgLength);

                // Reduce the data offset by the amount of bytes we have dropped from client data
                clientEntity.DataOffset -= msgLength;
            }

            Finalize(QueueType.Receive);
        }

        private void ProcessQueue(QueueType type)
        {
            BlockingCollection<QueuedMessage> queue = (type == QueueType.Send) ? sendCollection : recvCollection;

            if (type == QueueType.Send)
                sendProcessing = true;
            else
                recvProcessing = true;

            queue.CompleteAdding();

            QueuedMessage queuedMsg = null;

            while (queue.TryTake(out queuedMsg))
            {
                byte[] sendBuffer = new byte[queuedMsg.Message.Length];

                if (type == QueueType.Send)
                {
                    string clientTag = "Auth Server";

                    var clientEntity = queuedMsg.Client.GetEntity();

                    if (queuedMsg.Client is UploadClient)
                        clientTag = "Upload Server";
                    else if (queuedMsg.Client is GameClient)
                        clientTag = $"Game Client {clientEntity.IP}:{clientEntity.Port}@{((GameClientEntity)clientEntity).Info.AccountName.String}";

                    sendBuffer = queuedMsg.Message.Data;

                    if (queuedMsg.Client is GameClient)
                        SendCipher.Encode(queuedMsg.Message.Data, sendBuffer, sendBuffer.Length);

                    if (debugPackets)
                    {
                        string packetDmp = ((Packet)queuedMsg.Message).DumpToString();

                        notificationSVC.WriteMarkup($"[bold orange3]Sending {queuedMsg.Message.GetType().Name} ({queuedMsg.Message.Data.Length} bytes) to the {clientTag}[/]\n\n{packetDmp}");
                    }

                    queuedMsg.Client.Send(sendBuffer);
                }
                else
                {
                    if (queuedMsg.Client is AuthClient)
                        authActionSVC.Execute(queuedMsg.Client, queuedMsg.Message);
                    else if (queuedMsg.Client is GameClient)
                        gameActionSVC.Execute(queuedMsg.Client, queuedMsg.Message);
                    else if (queuedMsg.Client is UploadClient)
                        uploadActionSVC.Execute(queuedMsg.Client, queuedMsg.Message);
                }
            }

            if (type == QueueType.Send)
                sendProcessing = false;
            else
                recvProcessing = false;
        }

    }
}
