using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using System.Net;
using System.Net.Sockets;
using Network.Packets;
using Notification;
using Network;

namespace Navislamia.Network.Objects
{
    public interface IClient
    {
        public int Connect(IPEndPoint ep);
    }


    public class Client : IClient
    {
        public INotificationService NotificationService;
        public INetworkService NetworkService;

        public uint MsgVersion = 0x070300;

        public Client(Socket socket, int length, INotificationService notificationService, INetworkService networkService)
        {
            Socket = socket;
            BufferLen = length;
            NotificationService = notificationService;
            NetworkService = networkService;
        }

        public int DataLength => Data?.Length ?? -1;

        public int BufferLen = -1;

        public byte[] Data;

        public uint MessageID
        {
            get
            {
                if (Data == null)
                    return 0;

                Span<byte> data = Data;

                return BitConverter.ToUInt16(data.Slice(4, 2));
            }
        }

        public Socket Socket = null;

        public bool Connected => Socket?.Connected ?? false;

        public int Connect(IPEndPoint ep)
        {
            try
            {
                Socket.Connect(ep);
            }
            catch (Exception ex)
            {
                NotificationService.WriteException(ex);

                return 1;
            }

            return 0;
        }

        public virtual void Send(ISerializablePacket msg, bool beginReceive = true) { }

        public virtual void Receive() { }
    }
}
