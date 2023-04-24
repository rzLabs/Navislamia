using Navislamia.Network.Packets.Actions.Interfaces;
using Network;
using System.Net.Sockets;
using Configuration;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Notification;

namespace Navislamia.Network.Entities
{
    public class UploadClient : Client
    {
        private readonly NetworkOptions _networkOptions;
        private readonly LogOptions _logOptions;
        
        public UploadClient(Socket socket, int length, INotificationService notificationService,
            INetworkService networkService, IUploadActionService uploadActionService,
            IOptions<NetworkOptions> networkOptions, IOptions<LogOptions> logOptions) : base(socket, length, 
            notificationService, networkService, null, uploadActionService, null, 
            networkOptions, logOptions)
        {
            Data = new byte[BufferLen];
        }

    }
}
