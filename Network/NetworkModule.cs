using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Configuration;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Network.Entities;
using Navislamia.Network.Interfaces;
using Navislamia.Network.Packets.Actions;
using Navislamia.Network.Packets.Actions.Interfaces;
using Navislamia.Network.Packets.Auth;
using Navislamia.Network.Packets.Upload;
using Navislamia.Notification;
using Network;

namespace Navislamia.Network
{
    public class NetworkModule : INetworkService
    {
        private readonly INotificationService _notificationSvc;

        private TcpListener _listener = null;

        private int _bufferLength = 1024;

        private List<GameClient> _connections = new();

        private AuthClient _auth;
        private UploadClient _upload;

        private readonly IAuthActionService _authActionSvc;
        private readonly IGameActionService _gameActionSvc;
        private readonly IUploadActionService _uploadActionSvc;
        private readonly NetworkOptions _networkOptions;
        private readonly IOptions<NetworkOptions> _networkTmpOptions; // Temporary to merge options pattern. Refactor Clients to be injectable
        private readonly ServerOptions _serverOptions;
        /// <summary>
        /// Game clients that have not been authorized yet
        /// </summary>
        public Dictionary<string, IClient> AuthAccounts { get; set; } = new Dictionary<string, IClient>();

        /// <summary>
        /// Game clients that have been authorized and are now only the gameserver
        /// </summary>
        public Dictionary<string, IClient> GameClients { get; set; } = new Dictionary<string, IClient>();

        public int PlayerCount => GameClients.Count;

        public bool Ready => ((AuthClientEntity)_auth.Entity).Ready && ((UploadClientEntity)_upload.Entity).Ready;

        public AuthClient AuthClient => _auth;

        public UploadClient UploadClient => _upload;

        public NetworkModule() { }

        public NetworkModule(IOptions<NetworkOptions> networkOptions, IOptions<ServerOptions> serverOptions, INotificationService notificationService)
        {
            _notificationSvc = notificationService;
            _networkOptions = networkOptions.Value;
            _networkTmpOptions = networkOptions;
            

            _authActionSvc = new AuthActions(_notificationSvc, this);
            _gameActionSvc = new GameActions(networkOptions, _notificationSvc, this);
            _uploadActionSvc = new UploadActions(_notificationSvc);
        }

        public int Initialize()
        {
            if (ConnectToAuth() > 0)
                return 1;

            _notificationSvc.WriteDebug(new[] { "Connected to Auth server successfully!" }, true);

            if (SendGsInfoToAuth() > 0)
                return 2;

            if (ConnectToUpload() > 0)
                return 3;

            _notificationSvc.WriteDebug(new[] { "Connected to Upload server successfully!" }, true);

            if (SendInfoToUpload() > 0)
                return 4;

            return 0;
        }

        private int ConnectToAuth()
        {
            string addrStr = _networkOptions.Ip;
            int port = _networkOptions.Port;

            if (string.IsNullOrEmpty(addrStr) || port == 0)
            {
                _notificationSvc.WriteError("Invalid network auth.io.ip configuration! Review your Configuration.json!");
                return 1;
            }

            IPAddress addr;

            if (!IPAddress.TryParse(addrStr, out addr))
            {
                _notificationSvc.WriteError($"Failed to parse auth.io.ip: {addrStr}");
                return 1;
            }

            IPEndPoint authEp = new IPEndPoint(addr, port);

            var authSock = new Socket(addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            int status = 0;

            try
            {
                _auth = new AuthClient(_networkTmpOptions, _notificationSvc, _authActionSvc, null, null);
                _auth.Create(authSock);

                status = _auth.Connect(authEp);
            }
            catch (Exception ex)
            {
                _notificationSvc.WriteException(ex);

                status = 1;
            }

            if (status == 1)
            {
                _notificationSvc.WriteError("Failed to connect to the auth server!");

                return status;
            }

            return 0;
        }

        private int SendGsInfoToAuth()
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

                _auth.PendMessage(msg);
            }
            catch (Exception ex)
            {
                _notificationSvc.WriteException(ex);

                return 1;
            }

            return 0;
        }

        private int ConnectToUpload()
        {
            string addrStr = _networkOptions.UploadIp;
            int port = _networkOptions.UploadPort;

            if (string.IsNullOrEmpty(addrStr) || port == 0)
            {
                _notificationSvc.WriteError("Invalid network io.upload.ip configuration! Review your Configuration.json!");
                return 1;
            }

            IPAddress addr;

            if (!IPAddress.TryParse(addrStr, out addr))
            {
                _notificationSvc.WriteError($"Failed to parse io.upload.ip: {addrStr}");
                return 1;
            }

            IPEndPoint uploadEp = new IPEndPoint(addr, port);

            var uploadSock = new Socket(addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            int status = 0;

            try
            {
                _upload = new UploadClient(_networkTmpOptions, _notificationSvc, null, null, _uploadActionSvc);
                _upload.Create(uploadSock);

                status = _upload.Connect(uploadEp);
            }
            catch (Exception ex)
            {
                _notificationSvc.WriteException(ex);

                status = 1;
            }

            if (status == 1)
            {
                _notificationSvc.WriteError("Failed to connect to the upload server!");

                return 1;
            }

            return 0;
        }

        private int SendInfoToUpload()
        {
            try
            {
                var serverName = _serverOptions.Name;
                var msg = new TS_SU_LOGIN(serverName);

                _upload.PendMessage(msg);
            }
            catch (Exception ex)
            {
                _notificationSvc.WriteException(ex);

                return 1;
            }

            return 0;
        }

        private int StartClientListener()
        {
            string addrStr = _networkOptions.Ip;
            ushort port = _networkOptions.Port;
            int backlog = _networkOptions.Backlog;

            IPAddress addr;

            if (!IPAddress.TryParse(addrStr, out addr))
            {
                _notificationSvc.WriteError($"Failed to parse io.ip: {addrStr}");
                return 1;
            }


            _listener = new TcpListener(addr, port);
            _listener.Start(backlog);
            _listener.BeginAcceptSocket(AttemptAcceptScoket, _listener);

            _notificationSvc.WriteSuccess(new string[] { "Game network started!", $"- [yellow]Listening at: {addrStr} : {port} with backlog of: {backlog}[/]\n" }, true);

            return 0;
        }

        private void AttemptAcceptScoket(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;

            Socket socket = listener.EndAcceptSocket(ar);

            socket.NoDelay = true;

            GameClient client = new GameClient(_networkTmpOptions, _notificationSvc, null, _gameActionSvc, null);
            client.Create(socket);

            _notificationSvc.WriteDebug($"Game client connected from: {client.Entity.IP}");

            client.Listen();
        }

        public int StartListener()
        {
            if (StartClientListener() > 0)
            {
                _notificationSvc.WriteError("Failed to start client listener!");
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
