using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using System.Net;
using System.Net.Sockets;

using Configuration;
using Navislamia.Notification;
using Serilog.Events;

using Network.Security;

using Navislamia.Network.Interfaces;
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
        public Dictionary<string, IClient> AuthAccounts { get; set; } = new Dictionary<string, IClient>();

        /// <summary>
        /// Game clients that have been authorized and are now only the gameserver
        /// </summary>
        public Dictionary<string, IClient> GameClients { get; set; } = new Dictionary<string, IClient>();

        public int PlayerCount => GameClients.Count;

        public bool Ready => ((AuthClientEntity)auth.Entity).Ready && ((UploadClientEntity)upload.Entity).Ready;

        public AuthClient AuthClient => auth;

        public UploadClient UploadClient => upload;

        public NetworkModule() { }

        public NetworkModule(IConfigurationService configurationService, INotificationService notificationService)
        {
            configSVC = configurationService;
            notificationSVC = notificationService;

            authActionSVC = new AuthActions(configSVC, notificationSVC, this);
            gameActionSVC = new GameActions(configSVC, notificationSVC, this);
            uploadActionSVC = new UploadActions(configSVC, notificationSVC);
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
            string addrStr = configSVC.Get<string>("io.auth.ip", "network", "127.0.0.1");
            short port = configSVC.Get<short>("io.auth.port", "network", 4502);

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

            int status = 0;

            try
            {
                auth = new AuthClient(configSVC, notificationSVC, authActionSVC, null, null);
                auth.Create(authSock);

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
                var idx = configSVC.Get<ushort>("index", "server", 0);
                var ip = configSVC.Get<string>("io.ip", "network", "127.0.0.1");
                var port = configSVC.Get<short>("io.port", "network", 4515);
                var name = configSVC.Get<string>("name", "server", "Navislamia");
                var screenshot_url = configSVC.Get<string>("screenshort.url", "server", "about:blank");
                var adult_server = configSVC.Get<bool>("adult", "server", false);

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
            string addrStr = configSVC.Get<string>("io.upload.ip", "network", "127.0.0.1");
            short port = configSVC.Get<short>("io.upload.port", "network", 4616);

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

            int status = 0;

            try
            {
                upload = new UploadClient(configSVC, notificationSVC, null, null, uploadActionSVC);
                upload.Create(uploadSock);

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
                var serverName = configSVC.Get<string>("name", "server", "Navislamia");

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
            string addrStr = configSVC.Get<string>("io.ip", "network", "0.0.0.0");
            short port = configSVC.Get<short>("io.port", "network", 4515);
            int backlog = configSVC.Get<int>("io.backlog", "network", 100);

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

            GameClient client = new GameClient(configSVC, notificationSVC, null, gameActionSVC, null);
            client.Create(socket);

            notificationSVC.WriteDebug($"Game client connected from: {client.Entity.IP}");

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

        public bool RegisterAccount(IClient client, string accountName)
        {
            if (GameClients.ContainsKey(accountName))
                return false;

            GameClients[accountName] = client;

            return true;
        }
    }
}
