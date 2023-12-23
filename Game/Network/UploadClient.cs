using Navislamia.Game.Network.Packets;
using Serilog;
using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Navislamia.Game.Network;

public class UploadClient : Client, IClient
{
    static ILogger _logger = Log.ForContext<UploadClient>();

    UploadActionService _uploadActionService;

    public UploadClient(Socket socket, UploadActionService uploadActionService) : base(new Connection(socket), _logger)
    {
        _uploadActionService = uploadActionService;

        ClientTag = $"Upload Server @{Connection.RemoteIp}:{Connection.RemotePort}";
    }

    public override void OnDataReceived(int bytesReceived)
    {
        var remainingData = bytesReceived;

        while (remainingData > Marshal.SizeOf<Header>())
        {
            var _header = new Header(Connection.Peek(Marshal.SizeOf<Header>()));
            var _isValidMsg = _header.Checksum == Header.CalculateChecksum(_header);

            if (!_isValidMsg)
            {
                _logger.Error("Invalid Message received from {clientTag} !!!", ClientTag);

                Connection.Disconnect();

                return;
            }

            var msgBuffer = Connection.Read((int)_header.Length);

            remainingData -= msgBuffer.Length;

            // Check for packets that haven't been defined yet (development)
            if (!Enum.IsDefined(typeof(UploadPackets), _header.ID))
            {
                _logger.Warning("Partial packet received from {clientTag} !!! ID: {id} Length: {length} Available Data: {remaining}", ClientTag, _header.ID, _header.Length, remainingData);
                continue;
            }

            IPacket msg = _header.ID switch
            {
                (ushort)UploadPackets.TS_US_LOGIN_RESULT => new Packet<TS_US_LOGIN_RESULT>(msgBuffer),

                _ => throw new Exception("Unknown Packet Type")
            };

            _logger.Debug("Packet Received from {clientTag} ID: {id} Length: {length} !!!", ClientTag, msg.ID, msg.Length);

            _uploadActionService.Execute(this, msg);
        }
    }
}