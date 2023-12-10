using System.Net;
using System.Net.Sockets;
using Navislamia.Network.Enums;

namespace Navislamia.Game.Network.Entities
{
    public class ClientEntity
    {
        public uint PacketVersion;

        public string IP
        {
            get
            {
                if (Socket is not null)
                {
                    IPEndPoint ep = Socket.RemoteEndPoint as IPEndPoint ?? Socket.LocalEndPoint as IPEndPoint;

                    return ep?.Address.ToString();
                }

                return null;
            }
        }

        public ushort Port
        {
            get
            {
                if (Socket is not null)
                {
                    IPEndPoint ep = null;

                    ep = Socket?.RemoteEndPoint as IPEndPoint ?? Socket?.LocalEndPoint as IPEndPoint;

                    return (ushort)ep.Port;
                }

                return 0;
            }
        }

        public bool Connected => (Socket is not null) ? Socket.Connected : false;

        public Socket Socket;

        public byte[] Data;

        public int DataOffset;

        public byte[] MessageBuffer;
        
        public bool Ready { get; set; }
        
        public ClientType Type { get; set; }

    }
    
}
