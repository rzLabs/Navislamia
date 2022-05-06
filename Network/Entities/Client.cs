using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using System.Net;
using System.Net.Sockets;
using Notification;
using Configuration;
using Network;
using Navislamia.Network.Packets;

namespace Navislamia.Network.Objects
{
    public class Client
    {
        public IConfigurationService ConfigurationService;
        public INotificationService NotificationService;
        public INetworkService NetworkService;

        public uint MsgVersion = 0x070300;

        public Client(Socket socket, int length, IConfigurationService configurationService, INotificationService notificationService, INetworkService networkService)
        {
            Socket = socket;
            BufferLen = length;
            ConfigurationService = configurationService;
            NotificationService = notificationService;
            NetworkService = networkService;
            DebugPackets = configurationService.Get<bool>("packet.debug", "Logs", false);
        }

        public int DataLength => Data?.Length ?? -1;

        public int BufferLen = -1;

        public bool DebugPackets = false;

        public byte[] Data;

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

        public virtual void Send(Packet msg, bool beginReceive = true) { }

        public virtual void Receive() { }
    }
}
