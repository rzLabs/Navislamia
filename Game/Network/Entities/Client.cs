using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Packets.Enums;
using Serilog;

namespace Navislamia.Game.Network.Entities;

public class Client
{
    private readonly ILogger _logger = Log.ForContext<Client>();

    public ClientType Type { get; set; }
    public Connection Connection { get; set; }
    public string ClientTag { get; set; }

    public virtual void OnDataSent(int bytesSent) { }

    public virtual void OnDisconnect() { }

    public virtual void OnDataReceived(int bytesReceived) { }

    public virtual void SendMessage(IPacket msg)
    {
        Connection.Send(msg.Data);
        _logger.Debug("{name} ({id}) Length: {length} sent to {clientTag}", 
            msg.StructName, msg.ID, msg.Length, Type);
    }
}