using Configuration;
using Navislamia.Network.Packets.Actions.Interfaces;
using Network;
using Notification;
using System.Net.Sockets;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;

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
