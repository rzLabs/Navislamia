using Navislamia.Network.Packets;
using Navislamia.Notification;
using System.Net.Sockets;
using System.Net;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Configuration;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Network.Enums;
using Navislamia.Network.Extensions;
using Navislamia.Network.Packets.Actions.Interfaces;
using Navislamia.Network.Packets.Auth;
using Navislamia.Network.Packets.Game;
using Navislamia.Network.Packets.Upload;
using Network.Security;


namespace Navislamia.Network.Entities
{
    public class ClientService<T> : IClientService<T> where T : ClientEntity, new()
    {
        private readonly INotificationService notificationSVC;
        private readonly NetworkOptions _networkOptions;
        private readonly LogOptions _logOptions;
        
        bool debugPackets;
        bool sendProcessing;
        bool recvProcessing;

        XRC4Cipher RecvCipher = new();
        XRC4Cipher SendCipher = new();

        IAuthActionService authActionSVC;
        IGameActionService gameActionSVC;
        IUploadActionService uploadActionSVC;
        
        ConcurrentQueue<ISerializablePacket> sendQueue= new();
        BlockingCollection<ISerializablePacket> sendCollection;
        ConcurrentQueue<ISerializablePacket> recvQueue = new();
        BlockingCollection<ISerializablePacket> recvCollection;
        
        public T Entity;

        public ClientService(IAuthActionService authActionService, IGameActionService gameActionService, 
            IUploadActionService uploadActionService, IOptions<LogOptions> logOptions, 
            INotificationService notificationService, IOptions<NetworkOptions> networkOptions)
        {
            notificationSVC = notificationService;
            _networkOptions = networkOptions.Value;
            _networkOptions = networkOptions.Value;
            _logOptions = logOptions.Value;
            
            debugPackets = _logOptions.PacketDebug;
            notificationSVC = notificationService;

            RecvCipher.SetKey(_networkOptions.CipherKey);
            SendCipher.SetKey(_networkOptions.CipherKey);

            authActionSVC = authActionService;
            uploadActionSVC = uploadActionService;
            gameActionSVC = gameActionService;
            sendCollection = new BlockingCollection<ISerializablePacket>(sendQueue);
            recvCollection = new BlockingCollection<ISerializablePacket>(recvQueue);

            Task.Run(() =>
            {
                while (true)
                {
                    if (sendCollection.IsAddingCompleted && !sendProcessing)
                        ProcessQueue(QueueType.Send);

                    if (sendCollection.IsCompleted)
                        sendCollection = new BlockingCollection<ISerializablePacket>(sendQueue);

                    Thread.Sleep(100); // TODO research required processing speed
                }
            });

            Task.Run(() =>
            {
                while (true)
                {
                    if (recvCollection.IsAddingCompleted && !recvProcessing)
                        ProcessQueue(QueueType.Receive);

                    if (recvCollection.IsCompleted)
                        recvCollection = new BlockingCollection<ISerializablePacket>(recvQueue);

                    Thread.Sleep(100); // TODO research required processing speed
                }
            });
        }

        public T GetEntity()
        {
            return Entity;
        }

        public void Create(Socket socket)
        {
            var bufferLen = _networkOptions.BufferSize;

            ClientType type = Entity switch
            {
                AuthClientEntity => ClientType.Auth,
                GameClientEntity => ClientType.Game,
                UploadClientEntity => ClientType.Upload,
                _ => ClientType.Unknown
            };

            T client = new T()
            {
                Socket = socket,
                Data = new byte[bufferLen],
                MessageBuffer = new byte[bufferLen],
                Type = type
            };

            Entity = client;
        }

        public int Connect(IPEndPoint ep)
        {
            try
            {
                Entity.Socket.Connect(ep);
            }
            catch (Exception ex)
            {
                notificationSVC.WriteError($"An error occured while attempting to connect to remote endpoint!");
                notificationSVC.WriteException(ex);

                return 1;
            }

            return 0;
        }

        public void Send(byte[] data)
        {
            try
            {
                Entity.Socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallback, Entity);
            }
            catch (Exception ex)
            {
                notificationSVC.WriteError($"An error occured while attempting to send data to connection! {Entity.IP}:{Entity.Port}");
                notificationSVC.WriteException(ex);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            T entity = (T)ar.AsyncState;
            entity.Socket.EndSend(ar);
            Listen();
        }

        public void Listen()
        {
            if (!Entity.Socket.Connected)
                return;

            try
            {
                Entity.Socket.BeginReceive(Entity.MessageBuffer, 0, Entity.MessageBuffer.Length, SocketFlags.None, ListenCallback, Entity);
            }
            catch (Exception ex)
            {
                notificationSVC.WriteError($"An error occured while attempting to read listen for data from connection! {Entity.IP}:{Entity.Port}");
                notificationSVC.WriteException(ex);
            }
        }

        public void PendMessage(ISerializablePacket msg)
        {
            PendSend(msg);
            Finalize(QueueType.Send);
        }

        private void ListenCallback(IAsyncResult ar)
        {
            T entity = (T)ar.AsyncState;

            if (!entity.Socket.Connected)
            {
                notificationSVC.WriteError($"Read attempted for closed connection! {Entity.IP}:{Entity.Port}");
                return;
            }

            try
            {
                int availableBytes = entity.Socket.EndReceive(ar);

                if (availableBytes == 0)
                {
                    Listen();
                }

                ProcessClientData(entity.MessageBuffer, availableBytes);
            }
            catch (Exception ex)
            {
                notificationSVC.WriteError($"An error occured while attempting to read data from connection! {Entity.IP}:{Entity.Port}");
                notificationSVC.WriteException(ex);
            }
        }
        
        private void Finalize(QueueType type)
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

        private void PendSend(ISerializablePacket msg)
        {
            if (!sendCollection.TryAdd(msg))
            {
                notificationSVC.WriteError($"Failed to add msg to send queue! ID: {msg.ID}");
            }
        }

        private void PendReceive(ISerializablePacket msg)
        {
            if (!recvCollection.TryAdd(msg))
            {
                notificationSVC.WriteError($"Failed to add msg to send queue! ID: {msg.ID}");
            }
        }

        private void ProcessClientData(byte[] data, int count)
        {
            if (Entity is GameClientEntity)
            {
                byte[] buffer = new byte[count];

                RecvCipher.Decode(data, buffer, count);

                Buffer.BlockCopy(buffer, 0, Entity.Data, Entity.DataOffset, count);
            }
            else
                Buffer.BlockCopy(data, 0, Entity.Data, Entity.DataOffset, count);

            // increase the offset by the amount of bytes we wrote to the client data
            Entity.DataOffset += count;

            // Process and queue messages to be read from the data
            while (Entity.DataOffset >= 4)
            {
                // Get a pointer to the client data
                Span<byte> clientData = Entity.Data;

                // Read the message length
                int msgLength = BitConverter.ToInt32(clientData.Slice(0, 4));

                // If the message length is invalid ignore this message and advance the buffer by 4 bytes
                if (msgLength < 0 || msgLength > Entity.DataOffset)
                {
                    notificationSVC.WriteWarning($"Invalid message received from {Entity.Type.EnumToString()} client @ {Entity.IP}!!! Packet Length: {msgLength} @ DataOffset: {Entity.DataOffset}");
                    notificationSVC.WriteWarning(Utilities.StringExt.ByteArrayToString(((Span<byte>)data).Slice(0, count).ToArray()));

                    // if msgLength is below 0, set it to 4, if it above offset, set to 4
                    msgLength = Math.Max(4, Math.Min(4, Math.Min(msgLength, Entity.DataOffset)));
                }
                else // process and queue the message data
                {                
                    byte[] msgBuffer = clientData.Slice(0, msgLength).ToArray();

                    PacketHeader header = Header.GetPacketHeader(msgBuffer);
                    ISerializablePacket msg = null;

                    if (!Enum.IsDefined(typeof(ClientPackets), header.ID))
                        notificationSVC.WriteWarning($"Undefined packet {header.ID} (Checksum: {header.Checksum}, Length: {header.Length}) received from {Entity.Type.EnumToString()} client {Entity.IP} [{Entity.Port}]");

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
                            notificationSVC.WriteWarning($"TM_NONE of {header.Length} received from client {Entity.IP}:{Entity.Port}");
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

                            notificationSVC.WriteMarkup($"[bold orange3]Receiving message from {Entity.IP}:{Entity.Port}[/]\n\n{packetDmp}");
                        }

                        PendReceive(msg);
                    }
                }

                // move the remaining bytes to the front of client data
                Buffer.BlockCopy(Entity.Data, msgLength, Entity.Data, 0, Entity.Data.Length - msgLength);

                // Reduce the data offset by the amount of bytes we have dropped from client data
                Entity.DataOffset -= msgLength;
            }

            Finalize(QueueType.Receive);
        }

        private void ProcessQueue(QueueType type)
        {
            var queue = type == QueueType.Send ? sendCollection : recvCollection;

            if (type == QueueType.Send)
                sendProcessing = true;
            else
                recvProcessing = true;

            queue.CompleteAdding();

            ISerializablePacket queuedMsg;

            while (queue.TryTake(out queuedMsg))
            {
                byte[] sendBuffer = new byte[queuedMsg.Length];

                if (type == QueueType.Send)
                {
                    string clientTag = $"{Entity.Type.EnumToString()} Server";

                    sendBuffer = queuedMsg.Data;

                    if (Entity.Type is ClientType.Game)
                        SendCipher.Encode(queuedMsg.Data, sendBuffer, sendBuffer.Length);

                    if (debugPackets)
                    {
                        string packetDmp = ((Packet)queuedMsg).DumpToString();

                        notificationSVC.WriteMarkup($"[bold orange3]Sending {queuedMsg.GetType().Name} ({queuedMsg.Data.Length} bytes) to the {clientTag}[/]\n\n{packetDmp}");
                    }

                    Entity.Socket.Send(sendBuffer);
                }
                else
                {
 
                    if (Entity.Type is ClientType.Auth)
                        authActionSVC.Execute(this as ClientService<AuthClientEntity>, queuedMsg);
                    else if (Entity.Type is ClientType.Game)
                        gameActionSVC.Execute(this as ClientService<GameClientEntity>, queuedMsg);
                    else if (Entity.Type is ClientType.Upload)
                        uploadActionSVC.Execute(this as ClientService<UploadClientEntity>, queuedMsg);
                }
            }

            if (type == QueueType.Send)
                sendProcessing = false;
            else
                recvProcessing = false;
        }

        public void SendResult(ushort id, ushort result, int value = 0)
        {
            PendMessage(new TS_SC_RESULT(id, result, value));
        }
    }
        
}



