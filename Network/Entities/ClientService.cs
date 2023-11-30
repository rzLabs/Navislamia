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
using Navislamia.Network.Packets.Auth;
using Navislamia.Network.Packets.Game;
using Navislamia.Network.Packets.Upload;
using Navislamia.Utilities;
using Network.Security;

namespace Navislamia.Network.Entities
{
    // TODO: Poll connections by configuration interval
    // TODO: Disconnect/Destroy

    public class ClientService<T> : IClientService<T> where T : ClientEntity, new()
    {
        private readonly INotificationModule _notificationSvc;
        private INetworkModule _networkModule;

        private readonly NetworkOptions _networkOptions;
        private readonly LogOptions _logOptions;

        private bool _sendProcessing;
        private bool _recvProcessing;

        private readonly XRC4Cipher _recvCipher = new();
        private readonly XRC4Cipher _sendCipher = new();

        private readonly ConcurrentQueue<ISerializablePacket> _sendQueue = new();
        private readonly ConcurrentQueue<ISerializablePacket> _recvQueue = new();
        private BlockingCollection<ISerializablePacket> _sendCollection;
        private BlockingCollection<ISerializablePacket> _recvCollection;

        private readonly CancellationTokenSource _sendCancelSource = new();
        private readonly CancellationTokenSource _receiveCancelSource = new();

        public T Entity;

        public ClientService(IOptions<LogOptions> logOptions, INotificationModule notificationModule, IOptions<NetworkOptions> networkOptions)
        {
            _notificationSvc = notificationModule;

            _networkOptions = networkOptions.Value;
            _logOptions = logOptions.Value;

            _recvCipher.SetKey(_networkOptions.CipherKey);
            _sendCipher.SetKey(_networkOptions.CipherKey);

            _sendCollection = new BlockingCollection<ISerializablePacket>(_sendQueue);
            _recvCollection = new BlockingCollection<ISerializablePacket>(_recvQueue);

            ProcessSendQueue(_sendCancelSource.Token);
            ProcessReceiveQueue(_receiveCancelSource.Token);
        }

        private Task ProcessSendQueue(CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (_sendCollection.IsAddingCompleted && !_sendProcessing)
                        ProcessQueue(QueueType.Send);

                    if (_sendCollection.IsCompleted)
                        _sendCollection = new BlockingCollection<ISerializablePacket>(_sendQueue);

                    Thread.Sleep(100); // TODO research required processing speed
                }
            }, cancellationToken);

            return null;
        }

        private Task ProcessReceiveQueue(CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (_recvCollection.IsAddingCompleted && !_recvProcessing)
                        ProcessQueue(QueueType.Receive);

                    if (_recvCollection.IsCompleted)
                        _recvCollection = new BlockingCollection<ISerializablePacket>(_recvQueue);

                    Thread.Sleep(100); // TODO research required processing speed
                }
            }, cancellationToken);

            return null;
        }

        public T GetEntity()
        {
            return Entity;
        }

        public void Create(INetworkModule networkModule, Socket socket)
        {
            _networkModule = networkModule;

            var bufferLen = _networkOptions.BufferSize;

            var type = this switch
            {
                ClientService<AuthClientEntity> => ClientType.Auth,
                ClientService<GameClientEntity> => ClientType.Game,
                ClientService<UploadClientEntity> => ClientType.Upload,
                _ => ClientType.Unknown
            };

            Entity = new T
            {
                Socket = socket,
                Data = new byte[bufferLen],
                MessageBuffer = new byte[bufferLen],
                Type = type
            };

        }

        public void Connect(IPEndPoint ep)
        {
            try
            {
                Entity.Socket.Connect(ep);
            }
            catch (Exception ex)
            {
                _notificationSvc.WriteError($"An error occured while attempting to connect to remote endpoint!");
                _notificationSvc.WriteException(ex);
            }
        }

        public void Disconnect()
        {
            if (Entity is null)
                return;

            if (Entity.Socket is null)
                return;

            Entity.Socket.Disconnect(false);

            _notificationSvc.WriteSuccess($"{Entity.Type.EnumToString()} client {Entity.IP}:{Entity.Port} has been disconnected!");

            Entity.Socket.Dispose();

            _sendCancelSource.Cancel();
            _receiveCancelSource.Cancel();
        }

        public void Send(byte[] data)
        {
            try
            {
                Entity.Socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallback, Entity);
            }
            catch (Exception ex)
            {
                _notificationSvc.WriteError($"An error occured while attempting to send data to connection! {Entity.IP}:{Entity.Port}");
                _notificationSvc.WriteException(ex);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            T entity = (T)ar.AsyncState;
            if (entity == null)
            {
                throw new Exception("Lost entity on SendCallback");
            }
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
                _notificationSvc.WriteError($"An error occured while attempting to read listen for data from connection! {Entity.IP}:{Entity.Port}");
                _notificationSvc.WriteException(ex);
            }
        }

        private void ListenCallback(IAsyncResult ar)
        {
            T entity = (T)ar.AsyncState;

            if (entity == null || !entity.Socket.Connected)
            {
                _notificationSvc.WriteError($"Read attempted for invalid or closed connection! {Entity.IP}:{Entity.Port}");
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
                _notificationSvc.WriteError($"An error occured while attempting to read data from connection! {Entity.IP}:{Entity.Port}");
                _notificationSvc.WriteException(ex);
            }
        }

        private void Finalize(QueueType type)
        {
            switch (type)
            {
                case QueueType.Send:
                    _sendCollection.CompleteAdding();
                    break;
                case QueueType.Receive:
                    _recvCollection.CompleteAdding();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, $"Could not finalise type {type}");
            }
        }

        private void PendSend(ISerializablePacket msg)
        {
            if (!_sendCollection.TryAdd(msg))
            {
                _notificationSvc.WriteError($"Failed to add msg to send queue! ID: {msg.ID}");
            }
        }

        private void PendReceive(ISerializablePacket msg)
        {
            if (!_recvCollection.TryAdd(msg))
            {
                _notificationSvc.WriteError($"Failed to add msg to send queue! ID: {msg.ID}");
            }
        }

        public void SendMessage(ISerializablePacket msg)
        {
            PendSend(msg);
            Finalize(QueueType.Send);
        }

        public void SendResult(ushort id, ushort result, int value = 0)
        {
            SendMessage(new TS_SC_RESULT(id, result, value));
        }

        private void ProcessClientData(byte[] data, int count)
        {
            if (Entity.Type is ClientType.Game)
            {
                var buffer = new byte[count];

                _recvCipher.Decode(data, buffer, count);

                Buffer.BlockCopy(buffer, 0, Entity.Data, Entity.DataOffset, count);
            }
            else
            {
                Buffer.BlockCopy(data, 0, Entity.Data, Entity.DataOffset, count);
            }

            // increase the offset by the amount of bytes we wrote to the client data
            Entity.DataOffset += count;

            // Process and queue messages to be read from the data
            while (Entity.DataOffset >= 4)
            {
                // Get a pointer to the client data
                Span<byte> clientData = Entity.Data;

                // Read the message length
                var msgLength = BitConverter.ToInt32(clientData[..4]);

                // If the message length is invalid ignore this message and advance the buffer by 4 bytes
                if (msgLength < 0 || msgLength > Entity.DataOffset)
                {
                    _notificationSvc.WriteWarning(
                        $"Invalid message received from {Entity.Type.EnumToString()} client @ {Entity.IP}:{Entity.Port}!!! Packet Length: {msgLength} @ DataOffset: {Entity.DataOffset}");
                    _notificationSvc.WriteWarning(data.ByteArrayToString()[..count]);

                    // if msgLength is below 0, set it to 4, if it above offset, set to 4
                    msgLength = Math.Max(4, Math.Min(4, Math.Min(msgLength, Entity.DataOffset)));
                }
                else // process and queue the message data
                {
                    var msgBuffer = clientData.Slice(0, msgLength).ToArray();
                    var header = Header.GetPacketHeader(msgBuffer);

                    if (Entity.Type is ClientType.Auth && !Enum.IsDefined(typeof(AuthPackets), header.ID) ||
                        Entity.Type is ClientType.Upload && !Enum.IsDefined(typeof(UploadPackets), header.ID) ||
                        Entity.Type is ClientType.Game && !Enum.IsDefined(typeof(GamePackets), header.ID))
                    {
                        _notificationSvc.WriteWarning(
                            $"Undefined packet {header.ID} (Checksum: {header.Checksum}, Length: {header.Length}) received from {Entity.Type.EnumToString()} client {Entity.IP}:{Entity.Port}");
                    }

                    ISerializablePacket msg = header.ID switch
                    {
                        // Auth
                        (ushort)AuthPackets.TS_AG_LOGIN_RESULT => new TS_AG_LOGIN_RESULT(msgBuffer),
                        (ushort)AuthPackets.TS_AG_CLIENT_LOGIN => new TS_AG_CLIENT_LOGIN(msgBuffer),

                        // Game
                        (ushort)GamePackets.TM_NONE => null,
                        (ushort)GamePackets.TM_CS_VERSION => new TM_CS_VERSION(msgBuffer),
                        (ushort)GamePackets.TS_CS_CHARACTER_LIST => new TS_CS_CHARACTER_LIST(msgBuffer),
                        (ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH => new TM_CS_ACCOUNT_WITH_AUTH(msgBuffer),
                        (ushort)GamePackets.TS_CS_REPORT => new TS_CS_REPORT(msgBuffer),

                        // Upload
                        (ushort)UploadPackets.TS_US_LOGIN_RESULT => new TS_US_LOGIN_RESULT(msgBuffer),
                        _ => throw new Exception("Unknown Packet Type")
                    };

                    // add message to the queue
                    if (msg is not null)
                    {
                        if (_logOptions.PacketDebug)
                        {
                            var packetDmp = ((Packet)msg).DumpToString();

                            _notificationSvc.WriteMarkup($"[bold orange3]Received message from {Entity.Type.EnumToString()} client {Entity.IP}:{Entity.Port}[/]\n\n{packetDmp}");
                        }

                        PendReceive(msg);
                    }
                    else
                        _notificationSvc.WriteWarning($"TM_NONE of {header.Length} received from client {Entity.IP}:{Entity.Port}");
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
            var queue = type == QueueType.Send ? _sendCollection : _recvCollection;

            if (type == QueueType.Send)
            {
                _sendProcessing = true;
            }
            else
            {
                _recvProcessing = true;
            }

            queue.CompleteAdding();

            ISerializablePacket queuedMsg;

            while (queue.TryTake(out queuedMsg))
            {
                if (type == QueueType.Send)
                {
                    var clientTag = Entity.Type.EnumToString();

                    switch (Entity.Type)
                    {
                        case ClientType.Auth:
                        case ClientType.Upload:
                            clientTag += " server";
                            break;

                        case ClientType.Game:
                            clientTag += " client";
                            break;
                    }

                    var sendBuffer = queuedMsg.Data;

                    if (Entity.Type is ClientType.Game)
                        _sendCipher.Encode(queuedMsg.Data, sendBuffer, sendBuffer.Length);

                    if (_logOptions.PacketDebug)
                    {
                        var packetDmp = ((Packet)queuedMsg).DumpToString();

                        _notificationSvc.WriteMarkup($"[bold orange3]Sending {queuedMsg.GetType().Name} ({queuedMsg.Data.Length} bytes) to the {clientTag}[/]\n\n{packetDmp}");
                    }

                    Send(sendBuffer);
                }
                else
                {
                    switch (Entity.Type)
                    {
                        case ClientType.Auth:
                            _networkModule.AuthActions.Execute(this as ClientService<AuthClientEntity>, queuedMsg);
                            break;
                        case ClientType.Game:
                            _networkModule.GameActions.Execute(this as ClientService<GameClientEntity>, queuedMsg);
                            break;
                        case ClientType.Upload:
                            _networkModule.UploadActions.Execute(this as ClientService<UploadClientEntity>, queuedMsg);
                            break;
                        case ClientType.Unknown:

                        default:
                            {
                                throw new ArgumentOutOfRangeException(nameof(type), type, $"Could not execute action for {type} client");
                            }
                    }
                }
            }

            if (type == QueueType.Send)
            {
                _sendProcessing = false;
            }
            else
            {
                _recvProcessing = false;
            }
        }
    }      
}



