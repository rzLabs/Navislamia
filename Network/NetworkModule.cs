using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using System.Net;
using System.Net.Sockets;

using Configuration;
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
        IConfigurationService configSVC;
        INotificationService notificationSVC;

        TcpListener listener = null;

        int BufferLength = 1024;

        List<GameClient> connections = new List<GameClient>();

        AuthClient auth = null;
        UploadClient upload = null;

        IAuthActionService authActionSVC;
        IGameActionService gameActionSVC;
        IUploadActionService uploadActionSVC;

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

        public NetworkModule() { }

        public NetworkModule(IConfigurationService configurationService, INotificationService notificationService)
        {
            configSVC = configurationService;
            notificationSVC = notificationService;

            authActionSVC = new AuthActions(configSVC, notificationSVC, this);
            gameActionSVC = new GameActions(configSVC, notificationSVC, this); ;
            uploadActionSVC = new UploadActions(configSVC, notificationSVC); ;
        }

        public int Initialize()
        {
            if (connectToAuth() > 0)
                return 1;

            notificationSVC.WriteDebug(new[] { "Connected to Auth server successfully!" }, true);

            if (sendGSInfoToAuth() > 0)
                return 2;

            if (connectToUpload() > 0)
                return 3;

            notificationSVC.WriteDebug(new[] { "Connected to Upload server successfully!" }, true);

            if (sendInfoToUpload() > 0)
                return 4;

            return 0;
        }

        int connectToAuth()
        {
            string addrStr = configSVC.Get<string>("io.auth.ip", "Network", "127.0.0.1");
            short port = configSVC.Get<short>("io.auth.port", "Network", 4502);

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

            BufferLength = configSVC.Get<int>("io.buffer_size", "Network", 32768);

            int status = 0;

            try
            {
                auth = new AuthClient(authSock, BufferLength, configSVC, notificationSVC, this, authActionSVC);
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

        int sendGSInfoToAuth()
        {
            try
            {
                var idx = configSVC.Get<ushort>("index", "Server", 0);
                var ip = configSVC.Get<string>("io.ip", "Network", "127.0.0.1");
                var port = configSVC.Get<short>("io.port", "Network", 4515);
                var name = configSVC.Get<string>("name", "Server", "Navislamia");
                var screenshot_url = configSVC.Get<string>("screenshort.url", "Server", "about:blank");
                var adult_server = configSVC.Get<bool>("adult", "Server", false);

                var msg = new TS_GA_LOGIN(idx, ip, port, name, screenshot_url, adult_server);

                auth.PendMessage(msg);
            }
            catch (Exception ex)
            {
                notificationSVC.WriteException(ex);

                return 1;
            }

            return 0;
        }
        
        int connectToUpload()
        {
            string addrStr = configSVC.Get<string>("io.upload.ip", "Network", "127.0.0.1");
            short port = configSVC.Get<short>("io.upload.port", "Network", 4616);

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

            BufferLength = configSVC.Get<int>("io.buffer_size", "Network", 32768);

            int status = 0;

            try
            {
                upload = new UploadClient(uploadSock, BufferLength, configSVC, notificationSVC, this, uploadActionSVC);
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

        int sendInfoToUpload()
        {
            try
            {
                var serverName = configSVC.Get<string>("name", "Server", "Navislamia");

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

        int startClientListener()
        {
            string addrStr = configSVC.Get<string>("io.ip", "Network", "0.0.0.0");
            short port = configSVC.Get<short>("io.port", "Network", 4515);
            int backlog = configSVC.Get<int>("io.backlog", "Network", 100);

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

            GameClient client = new GameClient(socket, BufferLength, configSVC, notificationSVC, this, gameActionSVC);

            notificationSVC.WriteDebug($"Game client connected from: {client.IP}");

            client.Listen();
        }

        public int StartListener()
        {
            if (startClientListener() > 0)
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
