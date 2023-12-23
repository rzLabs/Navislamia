using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Interfaces;
using Navislamia.Game.Network.Entities;

using Serilog;

namespace Navislamia.Game.Network
{
    public class Client : ClientEntity
    {
        ILogger _logger;

        internal string ClientTag;

        public Client(IConnection connection, ILogger logger) : base(connection)
        {
            _logger = logger;
        }

        public void Start()
        {
            Connection.OnDataSent = OnDataSent;
            Connection.OnDataReceived = OnDataReceived;
            Connection.OnDisconnected = OnDisconnect;

            Connection.Start();
        }

        public virtual void OnDataSent(int bytesSent)
        {
        }

        public virtual void OnDataReceived(int bytesReceived)
        {
        }

        public virtual void OnDisconnect()
        {
        }

        public void SendMessage(IPacket msg)
        {
            Connection.Send(msg.Data);

            _logger.Debug("Packet ID: {id} Length: {length} sent to {clientTag}", msg.ID, msg.Length, ClientTag);
        }
    }
}
