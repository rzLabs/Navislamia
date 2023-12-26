using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Packets.Enums;

namespace Navislamia.Game.Network.Entities;

public class Client
{
    public ClientType Type { get; set; }
    public Connection Connection { get; set; }
    public string ClientTag { get; set; }
    public bool IsAuthorized { get; set; } = false;

    public virtual void OnDataSent(int bytesSent) { }

    public virtual void OnDisconnect() { }

    public virtual void OnDataReceived(int bytesReceived) { }

    public virtual void OnDisconnect(string accountName) { }
    
    public virtual void SendMessage(IPacket msg)
    {
        Connection.Send(msg.Data);
        // _logger.LogDebug("{name} ({id}) Length: {length} sent to {clientTag}", 
        //     msg.StructName, msg.ID, msg.Length, Client.Type);
    }
}