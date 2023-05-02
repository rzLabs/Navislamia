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
using System.Threading;
using System.Threading.Tasks;
using Configuration;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Network.Interfaces;

namespace Navislamia.Network.Entities
{
    public class MessageQueue
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
                        processQueue(QueueType.Send);

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
                        processQueue(QueueType.Receive);

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

        public void LoadEncryptedBuffer(GameClient client, byte[] encryptedBuffer, int count)
        {
            var clientEntity = client.GetEntity();

            byte[] decodedBuffer = new byte[count];

            // decode what data we have received
            RecvCipher.Decode(encryptedBuffer, decodedBuffer, count);

            // move the decoded data into the client data at the current DataOffset
            Buffer.BlockCopy(decodedBuffer, 0, clientEntity.Data, clientEntity.DataOffset, count);

            // increase the offset by the amount of bytes we wrote to the client data
            clientEntity.DataOffset += decodedBuffer.Length;

            // Process and queue messages to be read from the data
            while (clientEntity.DataOffset >= 4)
            {
                // Get a pointer to the client data
                Span<byte> data = clientEntity.Data;

                // Read the message length
                int msgLength = BitConverter.ToInt32(data.Slice(0, 4));

                // If the message length is invalid ignore this message and advance the buffer by 4 bytes
                if (msgLength < 0 || msgLength > clientEntity.DataOffset)
                {
                    // Consider this could be auth/upload server client
                    notificationSVC.WriteWarning($"Invalid message received from client @ {clientEntity.IP}!!! Packet Length: {msgLength} @ DataOffset: {clientEntity.DataOffset}");
                    notificationSVC.WriteWarning(Utilities.StringExtensions.ByteArrayToString(((Span<byte>)decodedBuffer).Slice(0, decodedBuffer.Length).ToArray()));

                    // if msgLength is below 0, set it to 4, if it above offset, set to 4
                    msgLength = Math.Max(4, Math.Min(4, Math.Min(msgLength, clientEntity.DataOffset)));
                }
                else // process and queue the message data
                {
                    string clientName = client.Info?.AccountName.String ?? client.Entity.IP;

                    // buffer the message
                    byte[] msgBuffer = data.Slice(0, msgLength).ToArray();

                    PacketHeader header = Header.GetPacketHeader(msgBuffer);
                    ISerializablePacket msg = null;

                    if (!Enum.IsDefined(typeof(ClientPackets), header.ID))
                        notificationSVC.WriteWarning($"Undefined packet {header.ID} (Checksum: {header.Checksum}, Length: {header.Length}) received from game client {clientEntity.IP} [{clientEntity.Port}]@{client.Info.AccountName.String}");

                    switch (header.ID)
                    {
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

        public void LoadPlainBuffer(IClient client, byte[] messageBuffer, int count)
        {
            var clientEntity = client.GetEntity();

            string clientTag = "Auth Server Client";

            if (clientEntity is UploadClientEntity)
                clientTag = "Upload Server Client";
        
            // move the decoded data into the client data at the current client.DataOffset
            Buffer.BlockCopy(messageBuffer, 0, clientEntity.Data, clientEntity.DataOffset, count);

            // increase the offset by the amount of bytes we wrote to the client data
            clientEntity.DataOffset += count;

            // Process and queue messages to be read from the data
            while (clientEntity.DataOffset >= 4)
            {
                // Get a pointer to the client data
                Span<byte> data = clientEntity.Data;

                // Read the message length
                int msgLength = BitConverter.ToInt32(data.Slice(0, 4));

                // If the message length is invalid ignore this message and advance the buffer by 4 bytes
                if (msgLength < 0 || msgLength > clientEntity.DataOffset)
                {
                    notificationSVC.WriteWarning($"Invalid message received from {((clientEntity is AuthClientEntity) ? "Auth" : "Upload")} server!");
                    notificationSVC.WriteWarning(Utilities.StringExtensions.ByteArrayToString(((Span<byte>)messageBuffer).Slice(0, count).ToArray()));

                    // if msgLength is below 0, set it to 4, if it above offset, set to 4
                    msgLength = Math.Max(4, Math.Min(4, Math.Min(msgLength, clientEntity.DataOffset)));
                }
                else // process and queue the message data
                {
                    // buffer the message
                    byte[] msgBuffer = data.Slice(0, msgLength).ToArray();

                    PacketHeader header = Header.GetPacketHeader(msgBuffer);
                    ISerializablePacket msg = null;

                    if (!Enum.IsDefined(typeof(AuthPackets), header.ID) && !Enum.IsDefined(typeof(UploadPackets), header.ID))
                        notificationSVC.WriteWarning($"Undefined packet {header.ID} (Checksum: {header.Checksum}, Length: {header.Length}) received from {((clientEntity is AuthClientEntity) ? "Auth" : "Upload")} server!");


                    switch (header.ID)
                    {
                        case (int)AuthPackets.TS_AG_LOGIN_RESULT:
                            msg = new TS_AG_LOGIN_RESULT(msgBuffer);
                            break;

                        case (int)AuthPackets.TS_AG_CLIENT_LOGIN:
                            msg = new TS_AG_CLIENT_LOGIN(msgBuffer);
                            break;

                        #region Upload Packets

                        case (int)UploadPackets.TS_US_LOGIN_RESULT:
                            msg = new TS_US_LOGIN_RESULT(msgBuffer);
                            break;

                        #endregion
                    }

                    // add message to the queue
                    if (msg is not null)
                    {
                        PendReceive(client, msg);

                        if (debugPackets)
                        {
                            string packetDmp = ((Packet)msg).DumpToString();

                            notificationSVC.WriteMarkup($"[bold orange3]Receiving message from {clientTag}[/]\n\n{packetDmp}");
                        }
                    }
                }

                // move the remaining bytes to the front of client data
                Buffer.BlockCopy(clientEntity.Data, msgLength, clientEntity.Data, 0, clientEntity.Data.Length - msgLength);

                // Reduce the data offset by the amount of bytes we have dropped from client data
                clientEntity.DataOffset -= msgLength;
            }

            Finalize(QueueType.Receive);
        }

        void processQueue(QueueType type)
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
