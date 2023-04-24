using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using System.Net;
using System.Net.Sockets;

using Configuration;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Notification;
using Serilog.Events;

using Network.Security;

using Navislamia.Network.Packets;
using Navislamia.Network.Packets.Upload;
using Navislamia.Network.Packets.Auth;
using Navislamia.Network.Packets.Actions;
using Navislamia.Network.Packets.Actions.Interfaces;
using Navislamia.Network.Entities;

namespace Network
{
    public class NetworkModule : INetworkService
    {
        INotificationService notificationSVC;

        TcpListener listener = null;

        int BufferLength = 1024;

        List<GameClient> connections = new List<GameClient>();

        AuthClient auth = null;
        UploadClient upload = null;

        IAuthActionService authActionSVC;
        IGameActionService gameActionSVC;
        IUploadActionService uploadActionSVC;

        private readonly NetworkOptions _networkOptions;
        private readonly ServerOptions _serverOptions;

        public NetworkModule() { }

        public NetworkModule(IOptions<NetworkOptions> networkOptions, IOptions<ServerOptions> serverOptions, INotificationService notificationService)
        {
            notificationSVC = notificationService;
            _networkOptions = networkOptions.Value;
            _serverOptions = serverOptions.Value;
            
            authActionSVC = new AuthActions(notificationSVC, this);
            gameActionSVC = new GameActions(networkOptions, notificationSVC, this); ;
            uploadActionSVC = new UploadActions(notificationSVC); ;
        }
        /// <summary>
        /// Game clients that have not been authorized yet
        /// </summary>
        public Dictionary<string, Client> AuthAccounts { get; set; } = new Dictionary<string, Client>();

        /// <summary>
        /// Game clients that have been authorized and are now only the gameserver
        /// </summary>
        public Dictionary<string, Client> GameClients { get; set; } = new Dictionary<string, Client>();

        public int PlayerCount => GameClients.Count;

        public bool Ready => auth.Ready && upload.Ready;

        public AuthClient AuthClient => auth;

        public UploadClient UploadClient => upload;


        public int Initialize()
        {
            if (ConnectToAuth() > 0)
                return 1;

            notificationSVC.WriteDebug(new[] { "Connected to Auth server successfully!" }, true);

            if (SendGsInfoToAuth() > 0)
                return 2;

            if (ConnectToUpload() > 0)
                return 3;

            notificationSVC.WriteDebug(new[] { "Connected to Upload server successfully!" }, true);

            if (SendInfoToUpload() > 0)
                return 4;

            return 0;
        }

        int ConnectToAuth()
        {
            string addrStr = _networkOptions.Ip;
            int port = _networkOptions.Port;

            if (string.IsNullOrEmpty(addrStr) || port == 0)
            {
                notificationSVC.WriteError("Invalid network auth.io.ip configuration! Review your Configuration.json!");
                return 1;
            }

            IPAddress addr;

            if (!IPAddress.TryParse(addrStr, out addr))
            {
                notificationSVC.WriteError($"Failed to parse auth.io.ip: {addrStr}");
                return 1;
            }

            IPEndPoint authEP = new IPEndPoint(addr, port);

            var authSock = new Socket(addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            BufferLength = _networkOptions.BufferSize;

            int status = 0;

            try
            {
                //TODO passed null for network and log options. Since this should be refactored to use DI, ill lave it like this for now
                auth = new AuthClient(authSock, BufferLength, notificationSVC, this, authActionSVC, null, null);
                status = auth.Connect(authEP);
            }
            catch (Exception ex)
            {
                notificationSVC.WriteException(ex);

                status = 1;
            }

            if (status == 1)
            {
                notificationSVC.WriteError("Failed to connect to the auth server!");

                return status;
            }

            return 0;
        }

        int SendGsInfoToAuth()
        {
            try
            {
                var index = _serverOptions.Index;
                var ip = _networkOptions.Ip;
                var port = _networkOptions.Port;
                var name = _serverOptions.Name;
                var screenshotUrl = _serverOptions.ScreenshotUrl;
                var isAdultServer = _serverOptions.IsAdultServer;

                var msg = new TS_GA_LOGIN(index, ip, port, name, screenshotUrl, isAdultServer);

                auth.PendMessage(msg);
            }
            catch (Exception ex)
            {
                notificationSVC.WriteException(ex);

                return 1;
            }

            return 0;
        }
        
        int ConnectToUpload()
        {
            string addrStr = _networkOptions.UploadIp;
            int port = _networkOptions.UploadPort;

            if (string.IsNullOrEmpty(addrStr) || port == 0)
            {
                notificationSVC.WriteError("Invalid network io.upload.ip configuration! Review your Configuration.json!");
                return 1;
            }

            IPAddress addr;

            if (!IPAddress.TryParse(addrStr, out addr))
            {
                notificationSVC.WriteError($"Failed to parse io.upload.ip: {addrStr}");
                return 1;
            }

            IPEndPoint uploadEP = new IPEndPoint(addr, port);

            var uploadSock = new Socket(addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            BufferLength = _networkOptions.BufferSize;

            int status = 0;

            try
            {
                //TODO passed null for network and log options. Since this should be refactored to use DI, ill lave it like this for now
                //If its needed before than inject IConfigurations and get the section via _configuration.GetSection("ConfigName").Value;
                upload = new UploadClient(uploadSock, BufferLength, notificationSVC, this, uploadActionSVC, null, null);
                status = upload.Connect(uploadEP);
            }
            catch (Exception ex)
            {
                notificationSVC.WriteException(ex);

                status = 1;
            }

            if (status == 1)
            {
                notificationSVC.WriteError("Failed to connect to the upload server!");

                return 1;
            }

            return 0;
        }

        int SendInfoToUpload()
        {
            try
            {
                var serverName = _serverOptions.Name;

                var msg = new TS_SU_LOGIN(serverName);

                upload.PendMessage(msg);
            }
            catch (Exception ex)
            {
                notificationSVC.WriteException(ex);

                return 1;
            }

            return 0;
        }

        int StartClientListener()
        {
            string addrStr = _networkOptions.Ip;
            int port = _networkOptions.Port;
            int backlog = _networkOptions.Backlog;

            IPAddress addr;

            if (!IPAddress.TryParse(addrStr, out addr))
            {
                notificationSVC.WriteError($"Failed to parse io.ip: {addrStr}");
                return 1;
            }


            listener = new TcpListener(addr, port);
            listener.Start(backlog);
            listener.BeginAcceptSocket(AttemptAcceptScoket, listener);

            notificationSVC.WriteSuccess(new string[] { "Game network started!", $"- [yellow]Listening at: {addrStr} : {port} with backlog of: {backlog}[/]\n" }, true);

            return 0;
        }

        private void AttemptAcceptScoket(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;

            Socket socket = listener.EndAcceptSocket(ar);

            socket.NoDelay = true;
            
            //TODO passed null for network and log options. Since this should be refactored to use DI, ill lave it like this for now
            GameClient client = new GameClient(socket, BufferLength, notificationSVC, this, gameActionSVC, null, null);

            notificationSVC.WriteDebug($"Game client connected from: {client.IP}");

            client.Listen();
        }

        public int StartListener()
        {
            if (StartClientListener() > 0)
            {
                notificationSVC.WriteError("Failed to start client listener!");
                return 1;
            }

            return 0;
        }

        public bool RegisterAccount(Client client, string accountName)
        {
            if (GameClients.ContainsKey(accountName))
                return false;

            GameClients[accountName] = client;

            return true;
        }
    }
}
