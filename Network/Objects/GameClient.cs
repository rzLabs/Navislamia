using Network;
using Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Objects
{
    public class GameClient : Client
    {
        public GameClient(Socket socket, int length, INotificationService notificationService, INetworkService networkService) : base(socket, length, notificationService, networkService) { }
    }
}
