using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Configuration;
using Navislamia.Game.Network.Entities.Actions;
using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Packets.Enums;
using Navislamia.Game.Network.Packets.Game;
using Navislamia.Network.Packets;
using Serilog;

namespace Navislamia.Game.Network.Entities;

public class GameClient : Client
{
    private readonly ILogger _logger = Log.ForContext<GameClient>();
    public GameClient(Socket socket, NetworkService networkService) : base(networkService, ClientType.Game)
    {
        Connection = new CipherConnection(socket, networkService.Options.CipherKey);
    }
    
    public void CreateClientConnection()
    {
        Connection.OnDataSent = OnDataSent;
        Connection.OnDataReceived = OnDataReceived;
        Connection.OnDisconnected = OnDisconnect;
        Connection.Start();;
    }

    public void SendResult(ushort id, ushort result, int value = 0)
    {
        var message = new Packet<TS_SC_RESULT>((ushort)GamePackets.TM_SC_RESULT, new TS_SC_RESULT(id, result, value));
        SendMessage(message);
    }
    
    public override void OnDataReceived(int bytesReceived)
    {
        var remainingData = bytesReceived;

        while (remainingData > Marshal.SizeOf<Header>())
        {
            var header = new Header(Connection.Peek(Marshal.SizeOf<Header>()));
            var isValidMsg = header.Checksum == Header.CalculateChecksum(header);

            if (header.Length > remainingData)
            {
                _logger.Warning(
                    "Partial packet received from {clientTag} !!! ID: {id} Length: {length} Available Data: {remaining}",
                    ClientTag, header.ID, header.Length, remainingData);

                return;
            }

            if (!isValidMsg)
            {
                _logger.Error("Invalid Message received from {clientTag} !!!", ClientTag);
                Connection.Disconnect();
                throw new Exception($"Invalid Message recieved from {ClientTag}");
            }

            var msgBuffer = Connection.Read((int)header.Length);

            remainingData -= msgBuffer.Length;

            // Check for packets that haven't been defined yet (development)
            if (!Enum.IsDefined(typeof(GamePackets), header.ID))
            {
                _logger.Debug("Undefined packet ID: {id} Length: {length}) received from {clientTag}", header.ID, header.Length, ClientTag);
                continue;
            }

            // TM_NONE is a dummy packet sent by the clientService for...."reasons"
            if (header.ID == (ushort)GamePackets.TM_NONE)
            {
                _logger.Debug("{name} ({id}) Length: {length} received from {clientTag}", "TM_NONE", header.ID, header.Length, ClientTag);
                continue;
            }

            IPacket msg = header.ID switch
            {
                //(ushort)GamePackets.TM_NONE => null,
                (ushort)GamePackets.TM_CS_VERSION => new Packet<TM_CS_VERSION>(msgBuffer),
                (ushort)GamePackets.TM_CS_CHARACTER_LIST => new Packet<TS_CS_CHARACTER_LIST>(msgBuffer),
                (ushort)GamePackets.TM_CS_CREATE_CHARACTER => new Packet<TS_CS_CREATE_CHARACTER>(msgBuffer),
                (ushort)GamePackets.TM_CS_CHECK_CHARACTER_NAME => new Packet<TS_CS_CHECK_CHARACTER_NAME>(msgBuffer),
                (ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH => new Packet<TM_CS_ACCOUNT_WITH_AUTH>(msgBuffer),
                (ushort)GamePackets.TM_CS_REPORT => new Packet<TS_CS_REPORT>(msgBuffer),

                _ => throw new Exception("Unknown Packet Type")
            };

            _logger.Debug("{name} ({id}) Length: {length} received from {clientTag}", msg.StructName, msg.ID, msg.Length, ClientTag);

            Actions.Execute(this, msg);
        }
    }
}