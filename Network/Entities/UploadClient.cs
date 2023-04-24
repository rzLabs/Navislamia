using Configuration;
using Navislamia.Network.Enums;
using Navislamia.Network.Packets;
using Navislamia.Network.Packets.Actions;
using Navislamia.Network.Packets.Actions.Interfaces;
using Navislamia.Network.Packets.Upload;
using Network;
using Navislamia.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Entities
{
    public class UploadClient : Client
    {
        public UploadClient(Socket socket, int length, IConfigurationService configurationService, INotificationService notificationService, INetworkService networkService, IUploadActionService uploadActionService) : base(socket, length, configurationService, notificationService, networkService, null, uploadActionService, null)
        {
            Data = new byte[BufferLen];
        }

    }
}
