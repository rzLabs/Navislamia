using Navislamia.Network.Enums;
using Navislamia.Network.Packets.Game;
using Navislamia.Network.Packets;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System;
using Navislamia.Network.Packets.Actions;
using Navislamia.Game.Models;
using Navislamia.Network.Packets.Auth;
using Navislamia.Network.Packets.Upload;
using Serilog;

namespace Navislamia.Game.Network.Entities;

public class GameClientService : ClientEntity
{
    ILogger _logger = Log.ForContext<AuthClientService>();

    GameActionService _gameActionService;

    public ConnectionInfo Info { get; set; } = new ConnectionInfo();

    public GameClientService(Socket socket, string cipherKey, GameActionService gameActionService) : base(new CipherConnection(socket, cipherKey))
    {
        _gameActionService = gameActionService;
    }

    public string ClientTag => $"Game Client @{Connection.RemoteIp}:{Connection.RemotePort}";

    public void Start()
    {
        Connection.OnDataReceived = OnDataReceived;
        Connection.OnDataSent = OnDataSent;
        Connection.OnDisconnected = OnDisconnect;

        Connection.Start();
    }

    public void SendMessage(IPacket msg)
    {
        Connection.Send(msg.Data);
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

        int _charInfoOffset = Marshal.SizeOf<Header>() + _packetStructLength;

        foreach (var _character in characterList)
        {
            Buffer.BlockCopy(_character.StructToByte(), 0, _packet.Data, _charInfoOffset, _lobbyCharacterStructLength);

            _charInfoOffset += _lobbyCharacterStructLength;
        }

        SendMessage(_packet);
    }

    private void OnDataReceived(int bytesReceived)
    {
        var remainingData = bytesReceived;

        while (remainingData > Marshal.SizeOf<Header>())
        {
            var _header = new Header(Connection.Peek(Marshal.SizeOf<Header>()));
            var _isValidMsg = _header.Checksum == Header.CalculateChecksum(_header);

            if (_header.Length > remainingData)
            {
                _logger.Warning($"Partial packet received from {ClientTag} !!! ID: {_header.ID} Length: {_header.Length} Available Data: {remainingData}");

                return;
            }

            if (!_isValidMsg)
            {
                _logger.Error($"Invalid Message received from {ClientTag}");

                Connection.Disconnect();

                return;
            }

            var msgBuffer = Connection.Read((int)_header.Length);

            remainingData -= msgBuffer.Length;

            // Check for packets that haven't been defined yet (development)
            if (!Enum.IsDefined(typeof(GamePackets), _header.ID))
            {
                _logger.Warning($"Undefined packet {_header.ID} (Checksum: {_header.Checksum}, Length: {_header.Length}) received from {ClientTag}");
                continue;
            }

            // TM_NONE is a dummy packet sent by the client for...."reasons"
            if (_header.ID == (ushort)GamePackets.TM_NONE)
            {
                _logger.Debug($"TM_NONE received from {ClientTag}");

                continue;
            }

            IPacket msg = _header.ID switch
            {
                // Game
                //(ushort)GamePackets.TM_NONE => null,
                (ushort)GamePackets.TM_CS_VERSION => new Packet<TM_CS_VERSION>(msgBuffer),
                (ushort)GamePackets.TS_CS_CHARACTER_LIST => new Packet<TS_CS_CHARACTER_LIST>(msgBuffer),
                (ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH => new Packet<TM_CS_ACCOUNT_WITH_AUTH>(msgBuffer),
                (ushort)GamePackets.TS_CS_REPORT => new Packet<TS_CS_REPORT>(msgBuffer),

                _ => throw new Exception("Unknown Packet Type")
            };

            _logger.Debug($"Packet Received from {ClientTag} ID: {_header.ID} Length: {_header.Length} !!!");

            _gameActionService.Execute(this, msg);
        }
    }

    private void OnDataSent(int bytesSent)
    {
    }

    private void OnDisconnect()
    {
        _gameActionService.RemoveGameClient(Info.AccountName, this);
    }

}