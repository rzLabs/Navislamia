using Navislamia.Game.Network.Packets;
using Serilog;
using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Navislamia.Game.Network.Packets.Actions;

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
                _logger.Debug("Undefined packet ID: {id} Length: {length}) received from {clientTag}", _header.ID, _header.Length, ClientTag);
                continue;
            }

            IPacket msg = _header.ID switch
            {
                (ushort)UploadPackets.TS_US_LOGIN_RESULT => new Packet<TS_US_LOGIN_RESULT>(msgBuffer),

                _ => throw new Exception("Unknown Packet Type")
            };

            _logger.Debug("{name}({id}) Length: {length} received from {clientTag}", msg.StructName, msg.ID, msg.Length, ClientTag);

            _uploadActionService.Execute(this, msg);
        }
    }
}