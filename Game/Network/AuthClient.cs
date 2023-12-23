using Navislamia.Game.Network.Packets;
using Serilog;
using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Navislamia.Game.Network;

public class AuthClient : Client, IClient
{
    static ILogger _logger = Log.ForContext<AuthClient>();
    AuthActionService _authActionService;

    public AuthClient(Socket socket, AuthActionService authActionService) : base(new Connection(socket), _logger)
    {
        _authActionService = authActionService;

        ClientTag = $"Auth Server @{Connection.RemoteIp}:{Connection.RemotePort}";
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
            if (!Enum.IsDefined(typeof(AuthPackets), _header.ID))
            {
                _logger.Debug("Undefined packet ID: {id} Length: {length}) received from {clientTag}", _header.ID, _header.Length, ClientTag);
                continue;
            }

            IPacket msg = _header.ID switch
            {
                // Auth
                (ushort)AuthPackets.TS_AG_LOGIN_RESULT => new Packet<TS_AG_LOGIN_RESULT>(msgBuffer),
                (ushort)AuthPackets.TS_AG_CLIENT_LOGIN => new Packet<TS_AG_CLIENT_LOGIN>(msgBuffer),

                _ => throw new Exception("Unknown Packet Type")
            };

            _logger.Debug("Packet Received from {clientTag} ID: {id} Length: {length} !!!", ClientTag, msg.ID, msg.Length);

            _authActionService.Execute(this, msg);
        }
    }
}