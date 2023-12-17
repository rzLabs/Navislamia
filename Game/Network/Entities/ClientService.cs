using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Options;

using Configuration;

using Navislamia.Notification;
using Navislamia.Configuration.Options;
using Navislamia.Network.Enums;
using Navislamia.Network.Packets;
using Navislamia.Network.Packets.Auth;
using Navislamia.Network.Packets.Game;
using Navislamia.Network.Packets.Upload;
using Navislamia.Notification;
using Navislamia.Utilities;
using Network.Security;
using static Navislamia.Network.NetworkExtensions;
using static Navislamia.Network.Packets.PacketExtensions;
using System.Collections.Generic;
using Navislamia.Game.Network.Packets.Auth;
using Navislamia.Game.Models.Arcadia;

namespace Navislamia.Game.Network.Entities
{
    // TODO: Poll connections by configuration interval
    // TODO: Disconnect/Destroy

    public class ClientService<T> : IDisposable, IClientService<T> where T : ClientEntity, new()
    {
        private readonly INotificationModule _notificationSvc;
        private INetworkModule _networkModule;

        private readonly NetworkOptions _networkOptions;
        private readonly LogOptions _logOptions;

        private bool _sendProcessing;
        private bool _recvProcessing;

        private readonly XRC4Cipher _recvCipher = new();
        private readonly XRC4Cipher _sendCipher = new();

        private readonly ConcurrentQueue<IPacket> _sendQueue = new();
        private BlockingCollection<IPacket> _sendCollection;
        private readonly CancellationTokenSource _updateCancelSource = new();

        public T Entity;

        public ClientService(IOptions<LogOptions> logOptions, INotificationModule notificationModule, IOptions<NetworkOptions> networkOptions)
        {
            _notificationSvc = notificationModule;

            _networkOptions = networkOptions.Value;
            _logOptions = logOptions.Value;

            _recvCipher.SetKey(_networkOptions.CipherKey);
            _sendCipher.SetKey(_networkOptions.CipherKey);

            _sendCollection = new BlockingCollection<IPacket>(_sendQueue);
        }

        public void Dispose()
        {
            _updateCancelSource.Cancel();

            _recvCipher.Clear();
            _sendCipher.Clear();
            _sendQueue.Clear();
            _sendCollection = null;

            Entity = null;
        }
        

        private Task Update(CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    IPacket queuedMsg;

                    while (_sendCollection.TryTake(out queuedMsg))
                    {
                        var sendBuffer = queuedMsg.Data;

                        if (Entity.Type is ClientType.Game)
                            _sendCipher.Encode(queuedMsg.Data, sendBuffer, sendBuffer.Length);

                        if (_logOptions.PacketDebug)
                        {
                            var structDump = queuedMsg.DumpStructToString();
                            var dataDump = _notificationSvc.EscapeString(queuedMsg.DumpDataToHexString());

                            // TODO: Figure out a way to turn this back on. Had to disable because of TS_SC_CHARACTER_LIST
                            //_notificationSvc.WriteMarkup($"[bold orange3]Sending ({queuedMsg.Data.Length} bytes) to the {clientTag}[/]\n\n{structDump}\n{dataDump}");
                        }

                        Send(sendBuffer);
                    }

                    Thread.Sleep(50); // TODO: research required processing speed
                }
            }, cancellationToken);

            return null;
        }

        public T GetEntity()
        {
            return Entity;
        }

        public void Initialize(INetworkModule networkModule, Socket socket)
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

            Update(_updateCancelSource.Token);

            Task.Run(ListenLoop);
        }

        private void ListenLoop()
        {
            while (true)
            {
                try
                {
                    SocketError _socketError;

                    Span<byte> _receiveBuffer = new Span<byte>(Entity.MessageBuffer);

                    var availableBytes = Entity.Socket.Receive(_receiveBuffer, SocketFlags.None, out _socketError);

                    if (!Entity.Socket.IsConnected())
                    {
                        // If we've reached this point the client is no longer connected
                        if (Entity.Type is ClientType.Game)
                            _networkModule.RemoveGameClient(this as ClientService<GameClientEntity>);

                        break;
                    }

                    if (availableBytes > 0)
                    {
                        Entity.PendingDataLength = availableBytes;

                        // If we are receiving data from a Game Client we must decode it
                        if (Entity.Type is ClientType.Game)
                            _recvCipher.Decode(Entity.MessageBuffer, Entity.MessageBuffer, Entity.PendingDataLength);

                        while (Entity.PendingDataLength > Marshal.SizeOf<Header>())
                        {
                            Header _header = Entity.MessageBuffer.PeekHeader();

                            // If the packet length is more than the available packet this is obviously a bad read
                            if (_header.Length > Entity.PendingDataLength)
                                return;

                            // Get the data from the front of the Entity.MessageBuffer
                            byte[] msgBuffer = new byte[_header.Length];
                            Buffer.BlockCopy(Entity.MessageBuffer, 0, msgBuffer, 0, (int)_header.Length);

                            // Reduce the available data length and move the rest of the available data to the front of the buffer
                            Entity.PendingDataLength -= (int)_header.Length;
                            Buffer.BlockCopy(Entity.MessageBuffer, (int)_header.Length, Entity.MessageBuffer, 0, Entity.MessageBuffer.Length - (int)_header.Length);

                            // Check for packets that haven't been defined yet (development)
                            if (Entity.Type is ClientType.Auth && !Enum.IsDefined(typeof(AuthPackets), _header.ID) ||
                                Entity.Type is ClientType.Upload && !Enum.IsDefined(typeof(UploadPackets), _header.ID) ||
                                Entity.Type is ClientType.Game && !Enum.IsDefined(typeof(GamePackets), _header.ID))
                            {
                                _notificationSvc.WriteWarning($"Undefined packet {_header.ID} (Checksum: {_header.Checksum}, Length: {_header.Length}) received from {clientTag} @{Entity.IP}:{Entity.Port}");
                                continue;
                            }

                            // TM_NONE is a dummy packet sent by the client for...."reasons"
                            if (_header.ID == (ushort)GamePackets.TM_NONE)
                            {
                                _notificationSvc.WriteWarning($"Received TM_NONE from {clientTag} @P{Entity.IP}:{Entity.Port}");
                                continue;
                            }

                            IPacket msg = _header.ID switch
                            {
                                // Auth
                                (ushort)AuthPackets.TS_AG_LOGIN_RESULT => new Packet<TS_AG_LOGIN_RESULT>(msgBuffer),
                                (ushort)AuthPackets.TS_AG_CLIENT_LOGIN => new Packet<TS_AG_CLIENT_LOGIN>(msgBuffer),

                                // Game
                                //(ushort)GamePackets.TM_NONE => null,
                                (ushort)GamePackets.TM_CS_VERSION => new Packet<TM_CS_VERSION>(msgBuffer),
                                (ushort)GamePackets.TS_CS_CHARACTER_LIST => new Packet<TS_CS_CHARACTER_LIST>(msgBuffer),
                                (ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH => new Packet<TM_CS_ACCOUNT_WITH_AUTH>(msgBuffer),
                                (ushort)GamePackets.TS_CS_REPORT => new Packet<TS_CS_REPORT>(msgBuffer),

                                // Upload
                                (ushort)UploadPackets.TS_US_LOGIN_RESULT => new Packet<TS_US_LOGIN_RESULT>(msgBuffer),

                                _ => throw new Exception("Unknown Packet Type")
                            };

                            if (_logOptions.PacketDebug)
                            {
                                var structDump = msg.DumpStructToString();
                                var dataDump = _notificationSvc.EscapeString(msg.DumpDataToHexString());

                                _notificationSvc.WriteMarkup($"[bold orange3]Received ({msg.Length} bytes) from {clientTag} @{Entity.IP}:{Entity.Port}[/]\n\n{structDump}\n{dataDump}");
                            }

                            switch (Entity.Type)
                            {
                                case ClientType.Auth:
                                    _networkModule.AuthActions.Execute(this as ClientService<AuthClientEntity>, msg);
                                    break;
                                case ClientType.Game:
                                    _networkModule.GameActions.Execute(this as ClientService<GameClientEntity>, msg);
                                    break;
                                case ClientType.Upload:
                                    _networkModule.UploadActions.Execute(this as ClientService<UploadClientEntity>, msg);
                                    break;
                                case ClientType.Unknown:

                                default:
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(ClientType), Entity.Type, $"Could not execute action for {clientTag}");
                                    }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _notificationSvc.WriteError($"An error occured while attempting to read listen for data from {clientTag} @{Entity.IP}:{Entity.Port} !!!");
                    _notificationSvc.WriteException(ex);

                    break;
                }
            }
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

            _notificationSvc.WriteSuccess($"{clientTag} @{Entity.IP}:{Entity.Port} has been disconnected!");

            Entity.Socket.Dispose();

            _updateCancelSource.Cancel();
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
        }

        private void ListenCallback(IAsyncResult ar)
        {
            T entity = (T)ar.AsyncState;

            if (entity == null || !entity.Socket.IsConnected())
            {
                _notificationSvc.WriteError($"Client {clientTag} @{Entity.IP}:{Entity.Port} disconnected!!!");

                if (Entity.Type is ClientType.Game)
                {
                    _networkModule.RemoveGameClient(this as ClientService<GameClientEntity>);
                }

                return;
            }

            try
            {
                int availableBytes = entity.Socket.EndReceive(ar);

                if (availableBytes > 0)
                {
                    entity.PendingDataLength = availableBytes;

                    // If we are receiving data from a Game Client we must decode it
                    if (Entity.Type is ClientType.Game)
                        _recvCipher.Decode(entity.MessageBuffer, entity.MessageBuffer, entity.PendingDataLength);

                    while (entity.PendingDataLength > Marshal.SizeOf<Header>())
                    {
                        Header _header = Entity.MessageBuffer.PeekHeader();

                        // If the packet length is more than the available packet this is obviously a bad read
                        if (_header.Length > Entity.PendingDataLength)
                            return;

                        // Get the data from the front of the Entity.MessageBuffer
                        byte[] msgBuffer = new byte[_header.Length];
                        Buffer.BlockCopy(Entity.MessageBuffer, 0, msgBuffer, 0, (int)_header.Length);

                        // Reduce the available data length and move the rest of the available data to the front of the buffer
                        Entity.PendingDataLength -= (int)_header.Length;
                        Buffer.BlockCopy(Entity.MessageBuffer, (int)_header.Length, Entity.MessageBuffer, 0, Entity.MessageBuffer.Length - (int)_header.Length);

                        // Check for packets that haven't been defined yet (development)
                        if (entity.Type is ClientType.Auth && !Enum.IsDefined(typeof(AuthPackets), _header.ID) ||
                            entity.Type is ClientType.Upload && !Enum.IsDefined(typeof(UploadPackets), _header.ID) ||
                            entity.Type is ClientType.Game && !Enum.IsDefined(typeof(GamePackets), _header.ID))
                        {
                            _notificationSvc.WriteWarning($"Undefined packet {_header.ID} (Checksum: {_header.Checksum}, Length: {_header.Length}) received from {clientTag} @{Entity.IP}:{Entity.Port}");
                            continue;
                        }

                        // TM_NONE is a dummy packet sent by the client for...."reasons"
                        if (_header.ID == (ushort)GamePackets.TM_NONE)
                        {
                            _notificationSvc.WriteWarning($"Received TM_NONE from {clientTag} @P{entity.IP}:{entity.Port}");
                            continue;
                        }

                        IPacket msg = _header.ID switch
                        {
                            // Auth
                            (ushort)AuthPackets.TS_AG_LOGIN_RESULT => new Packet<TS_AG_LOGIN_RESULT>(msgBuffer),
                            (ushort)AuthPackets.TS_AG_CLIENT_LOGIN => new Packet<TS_AG_CLIENT_LOGIN>(msgBuffer),

                            // Game
                            //(ushort)GamePackets.TM_NONE => null,
                            (ushort)GamePackets.TM_CS_VERSION => new Packet<TM_CS_VERSION>(msgBuffer),
                            (ushort)GamePackets.TS_CS_CHARACTER_LIST => new Packet<TS_CS_CHARACTER_LIST>(msgBuffer),
                            (ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH => new Packet<TM_CS_ACCOUNT_WITH_AUTH>(msgBuffer),
                            (ushort)GamePackets.TS_CS_REPORT => new Packet<TS_CS_REPORT>(msgBuffer),

                            // Upload
                            (ushort)UploadPackets.TS_US_LOGIN_RESULT => new Packet<TS_US_LOGIN_RESULT>(msgBuffer),

                            _ => throw new Exception("Unknown Packet Type")
                        };

                        if (_logOptions.PacketDebug)
                        {
                            var structDump = msg.DumpStructToString();
                            var dataDump = _notificationSvc.EscapeString(msg.DumpDataToHexString());

                            _notificationSvc.WriteMarkup($"[bold orange3]Received ({msg.Length} bytes) from {clientTag} @{Entity.IP}:{Entity.Port}[/]\n\n{structDump}\n{dataDump}");
                        }

                        switch (Entity.Type)
                        {
                            case ClientType.Auth:
                                _networkModule.AuthActions.Execute(this as ClientService<AuthClientEntity>, msg);
                                break;
                            case ClientType.Game:
                                _networkModule.GameActions.Execute(this as ClientService<GameClientEntity>, msg);
                                break;
                            case ClientType.Upload:
                                _networkModule.UploadActions.Execute(this as ClientService<UploadClientEntity>, msg);
                                break;
                            case ClientType.Unknown:

                            default:
                                {
                                    throw new ArgumentOutOfRangeException(nameof(ClientType), Entity.Type, $"Could not execute action for {clientTag}");
                                }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _notificationSvc.WriteError($"An error occured while attempting to read data from connection! {Entity.IP}:{Entity.Port}");
                _notificationSvc.WriteException(ex);
            }


        }

        public void SendMessage(IPacket msg)
        {
            if (!_sendCollection.TryAdd(msg))
            {
                _notificationSvc.WriteError($"Failed to add msg to send queue! ID: {msg.ID}");
            }
        }

        public void SendResult(ushort id, ushort result, int value = 0)
        {
            SendMessage(new Packet<TS_SC_RESULT>((ushort)GamePackets.TM_SC_RESULT, new(id, result, value)));
        }

        public void SendCharacterList(List<LobbyCharacterInfo> characterList)
        {
            var _charCount = (ushort)characterList.Count;

            var _packetStructLength = Marshal.SizeOf<TS_SC_CHARACTER_LIST>();
            var _lobbyCharacterStructLength = Marshal.SizeOf<LobbyCharacterInfo>();
            var _lobbyCharacterBufferLength = _lobbyCharacterStructLength * characterList.Count;

            var _packet = new Packet<TS_SC_CHARACTER_LIST>(2004, new(0, 0, _charCount), (_packetStructLength + _lobbyCharacterBufferLength));

            int _charInfoOffset = Marshal.SizeOf<Header>() +_packetStructLength;

            foreach (var _character in characterList)
            {
                Buffer.BlockCopy(_character.StructToByte(), 0, _packet.Data, _charInfoOffset, _lobbyCharacterStructLength);

                _charInfoOffset += _lobbyCharacterStructLength;
            }

            SendMessage(_packet);
        }

        private string clientTag
        {
            get
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

                return clientTag;
            }
        }
    }      
}



