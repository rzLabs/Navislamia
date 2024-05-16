using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Packets.Enums;
using Navislamia.Game.Network.Packets.Game;
using Navislamia.Game.Network.Packets.Interfaces;
using Serilog;

namespace Navislamia.Game.Network.Clients;

public class GameClient : Client
{
    private readonly ILogger _logger = Log.ForContext<GameClient>();

    public readonly GameOptions Options;

    public GameClient(Socket socket, GameOptions gameOptions, NetworkService networkService) : base(networkService, ClientType.Game)
    {
        Options = gameOptions;

        Connection = new CipherConnection(socket, networkService.NetworkOptions.CipherKey);
    }
    
    public void CreateClientConnection()
    {
        Connection.OnDataSent = OnDataSent;
        Connection.OnDataReceived = OnDataReceived;
        Connection.OnDisconnected = OnDisconnect;
        Connection.Start();;
    }

    public override void SendMessage(IPacket msg)
    {
        _logger.Debug("{name} ({id}) Length: {length} sent to {clientTag}", msg.StructName, msg.Id, msg.Length, ClientTag);

        base.SendMessage(msg);
    }

    public void SendResult(ushort id, ushort result, int value = 0)
    {
        var message = new Packet<TS_SC_RESULT>((ushort)GamePackets.TM_SC_RESULT, new TS_SC_RESULT(id, result, value));
        SendMessage(message);
    }

    public void SendDisconnectDescription(DisconnectType type)
    {
        var message = new Packet<TS_SC_DISCONNECT_DESC>((ushort)GamePackets.TM_SC_DISCONNECT_DESC, new TS_SC_DISCONNECT_DESC(type));
        SendMessage(message);
    }
    
    public override void OnDataReceived(int bytesReceived)
    {
        var remainingData = bytesReceived;

        while (remainingData > Marshal.SizeOf<Header>())
        {
            var header = new Header(Connection.Peek(Marshal.SizeOf<Header>()));
            var isValidMsg = header.Checksum == header.CalculateChecksum();

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
                _logger.Debug("Undefined packet ({id}) Length: {length} received from {clientTag}", header.ID, header.Length, ClientTag);
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
                (ushort)GamePackets.TM_CS_LOGIN => new Packet<TS_LOGIN>(msgBuffer),
                (ushort)GamePackets.TM_CS_VERSION => new Packet<TS_CS_VERSION>(msgBuffer),
                (ushort)GamePackets.TM_CS_CHARACTER_LIST => new Packet<TS_CS_CHARACTER_LIST>(msgBuffer),
                (ushort)GamePackets.TM_CS_CREATE_CHARACTER => new Packet<TS_CS_CREATE_CHARACTER>(msgBuffer),
                (ushort)GamePackets.TM_CS_DELETE_CHARACTER => new Packet<TS_CS_DELETE_CHARACTER>(msgBuffer),
                (ushort)GamePackets.TM_CS_CHECK_CHARACTER_NAME => new Packet<TS_CS_CHECK_CHARACTER_NAME>(msgBuffer),
                (ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH => new Packet<TS_CS_ACCOUNT_WITH_AUTH>(msgBuffer),
                (ushort)GamePackets.TM_CS_REPORT => new Packet<TS_CS_REPORT>(msgBuffer),

                _ => throw new Exception("Unknown Packet Type")
            };

            _logger.Debug("{name} ({id}) Length: {length} received from {clientTag}", msg.StructName, msg.Id, msg.Length, ClientTag);

            Actions.Execute(this, msg);
        }
    }
}