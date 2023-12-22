using Navislamia.Network.Enums;
using Navislamia.Network.Packets;
using Navislamia.Network.Packets.Actions;
using Navislamia.Network.Packets.Auth;
using Navislamia.Network.Packets.Upload;
using Serilog;
using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Navislamia.Game.Network.Entities;

public class UploadClientService : ClientEntity   
{
    ILogger _logger = Log.ForContext<UploadClientService>();

    IUploadActionService _uploadActionService;

    public UploadClientService(Socket socket, IUploadActionService uploadActionService) : base(new Connection(socket))
    {
        _uploadActionService = uploadActionService;
    }

    public string ClientTag => $"Upload Server @{Connection.RemoteIp}:{Connection.RemotePort}";

    public void Start()
    {
        Connection.OnDataReceived = OnDataReceived;
        Connection.OnDataSent = OnDataSent;

        Connection.Start();
    }

    public void SendMessage(IPacket msg)
    {
        Connection.Send(msg.Data);
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
                _logger.Error($"Invalid Message received from {ClientTag} !!!");

                Connection.Disconnect();

                return;
            }

            var msgBuffer = Connection.Read((int)_header.Length);

            remainingData -= msgBuffer.Length;

            // Check for packets that haven't been defined yet (development)
            if (!Enum.IsDefined(typeof(UploadPackets), _header.ID))
            {
                _logger.Debug($"Undefined packet {_header.ID} (Checksum: {_header.Checksum}, Length: {_header.Length}) received from {ClientTag}");
                continue;
            }

            // TM_NONE is a dummy packet sent by the client for...."reasons"
            if (_header.ID == (ushort)GamePackets.TM_NONE)
            {
                _logger.Debug($"TM_NONE received from {ClientTag} Length: {_header.Length}");

                continue;
            }

            IPacket msg = _header.ID switch
            {
                (ushort)UploadPackets.TS_US_LOGIN_RESULT => new Packet<TS_US_LOGIN_RESULT>(msgBuffer),

                _ => throw new Exception("Unknown Packet Type")
            };

            _logger.Debug($"Packet Received from {ClientTag} ID: {_header.ID} Length: {_header.Length} !!!");

            _uploadActionService.Execute(this, msg);
        }
    }

    private void OnDataSent(int bytesSent)
    {

    }

}