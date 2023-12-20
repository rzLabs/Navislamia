using Configuration;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Network.Enums;
using Navislamia.Network.Packets;
using Navislamia.Network.Packets.Auth;
using Navislamia.Network.Packets.Game;
using Navislamia.Network.Packets.Upload;
using Navislamia.Notification;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using static Navislamia.Network.NetworkExtensions;
using static Navislamia.Network.Packets.PacketExtensions;

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

        public T Entity;

        public ClientService(IOptions<LogOptions> logOptions, INotificationModule notificationModule, IOptions<NetworkOptions> networkOptions)
        {
            _notificationSvc = notificationModule;

            _networkOptions = networkOptions.Value;
            _logOptions = logOptions.Value;
        }

        public void Dispose()
        {
            Entity = null;
        }
        

        public T GetEntity()
        {
            return Entity;
        }

        public void Initialize(INetworkModule networkModule, IConnection connection)
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
                Connection = connection,
                Type = type
            };

            // Setup events
            Entity.Connection.OnDataSent = onDataSent;
            Entity.Connection.OnDataReceived = onDataReceived;
            Entity.Connection.OnDisconnected = onDisconnected;

            Entity.Connection.Start();
        }

        private void onDisconnected()
        {
            _networkModule.RemoveGameClient(this as ClientService<GameClientEntity>);
        }

        private void onDataReceived(byte[] data)
        {
            var offset = 0;
            var remainingData = data.Length;

            while (remainingData > Marshal.SizeOf<Header>())
            {
                Header _header = data.PeekHeader();

                if (_header.Length > remainingData)
                {
                    _notificationSvc.WriteWarning($"Invalid Header Length!!! Length: {_header.Length} Available Data: {remainingData}");

                    return;
                }

                byte[] msgBuffer = new byte[_header.Length];
                Buffer.BlockCopy(data, offset, msgBuffer, 0, (int)_header.Length);

                remainingData -= msgBuffer.Length;

                if (remainingData > 0)
                {
                    Buffer.BlockCopy(data, msgBuffer.Length, data, 0, remainingData);
                }

                // Check for packets that haven't been defined yet (development)
                if (Entity.Type is ClientType.Auth && !Enum.IsDefined(typeof(AuthPackets), _header.ID) ||
                    Entity.Type is ClientType.Upload && !Enum.IsDefined(typeof(UploadPackets), _header.ID) ||
                    Entity.Type is ClientType.Game && !Enum.IsDefined(typeof(GamePackets), _header.ID))
                {
                    _notificationSvc.WriteWarning($"Undefined packet {_header.ID} (Checksum: {_header.Checksum}, Length: {_header.Length}) received from {clientTag} @{Entity.Connection.RemoteIp}:{Entity.Connection.RemotePort}");
                    continue;
                }

                // TM_NONE is a dummy packet sent by the client for...."reasons"
                if (_header.ID == (ushort)GamePackets.TM_NONE)
                {
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

                //if (_logOptions.PacketDebug)
                //{
                //    var structDump = msg.DumpStructToString();
                //    var dataDump = _notificationSvc.EscapeString(msg.DumpDataToHexString());

                //    _notificationSvc.WriteMarkup($"[bold orange3]Received ({msg.Length} bytes) from {clientTag} @{Entity.IP}:{Entity.Port}[/]\n\n{structDump}\n{dataDump}");
                //}

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

        private void onDataSent(int bytesSent)
        {
            
        }

        public void Disconnect()
        {
            if (Entity is null)
                return;

            if (Entity.Connection is null)
                return;

            Entity.Connection.Disconnect();

            _notificationSvc.WriteSuccess($"{clientTag} @{Entity.Connection.RemoteIp}:{Entity.Connection.RemotePort} has been disconnected!");

        }

        public void SendMessage(IPacket msg)
        {
            Entity.Connection.Send(msg.Data);
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



