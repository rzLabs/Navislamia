
using System.Net.Sockets;
using Notification;
using Network;
using Configuration;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Network.Packets.Actions.Interfaces;

namespace Navislamia.Network.Entities
{
    public class AuthClient : Client
    {
        private readonly NetworkOptions _networkOptions;
        private readonly LogOptions _logOptions;
        public AuthClient(Socket socket, int length, INotificationService notificationService,
            INetworkService networkService, IAuthActionService actions, IOptions<NetworkOptions> networkOptions,
            IOptions<LogOptions> logOptions) : base(socket, length, notificationService,
            networkService, actions, null, null, networkOptions, logOptions) 
        {
            Data = new byte[BufferLen];

        }


    }
}
