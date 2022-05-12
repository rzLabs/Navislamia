using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using System.Net;
using System.Net.Sockets;


using Navislamia.Network.Enums;
using Navislamia.Network.Packets;
using Navislamia.Network.Packets.Auth;
using Navislamia.Network.Packets.Actions;
using static Navislamia.Network.Packets.Extensions;

using Notification;
using Network;
using Configuration;
using Navislamia.Network.Packets.Actions.Interfaces;

namespace Navislamia.Network.Entities
{
    public class AuthClient : Client
    {
        public AuthClient(Socket socket, int length, IConfigurationService configurationService, INotificationService notificationService, INetworkService networkService, IAuthActionService actions) : base(socket, length, configurationService, notificationService, networkService, actions, null, null) 
        {
            Data = new byte[BufferLen];

        }


    }
}
