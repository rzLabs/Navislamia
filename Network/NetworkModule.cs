using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Configuration;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Network.Enums;
using Navislamia.Network.Entities;
using Navislamia.Network.Packets.Actions;
using Navislamia.Network.Packets.Auth;
using Navislamia.Network.Packets.Upload;
using Navislamia.Notification;

namespace Navislamia.Network
{
    public class NetworkModule : INetworkModule
    {
        private TcpListener _listener;
        private readonly IClientService<AuthClientEntity> _authService;
        private readonly IClientService<UploadClientEntity> _uploadService;

        private readonly INotificationService _notificationSvc;
        private readonly IOptions<NetworkOptions> _networkIOptions;
        private readonly NetworkOptions _networkOptions;
        private readonly ServerOptions _serverServerOptions;
        private readonly IOptions<LogOptions> _logOptions;

        public Dictionary<string, ClientService<GameClientEntity>> UnauthorizedGameClients { get; set; } = new();
        public Dictionary<string,  ClientService<GameClientEntity>> AuthorizedGameClients { get; set; } = new();

        public AuthActions AuthActions { get; }
        public GameActions GameActions { get; }
        public UploadActions UploadActions { get; }

        public NetworkModule(IClientService<AuthClientEntity> authService, IClientService<UploadClientEntity> uploadService,
            IOptions<NetworkOptions> networkOptions, INotificationService notificationService, IOptions<LogOptions> logOptions,
            IOptions<ServerOptions> serverOptions) 
        {
            _notificationSvc = notificationService;
            _authService = authService;
            _uploadService = uploadService;
            _networkIOptions = networkOptions;
            _networkOptions = networkOptions.Value;
            _serverServerOptions = serverOptions.Value;
            _logOptions = logOptions;

            AuthActions = new AuthActions(notificationService, this);
            GameActions = new GameActions(notificationService, this, _networkOptions);
            UploadActions = new UploadActions(notificationService);
        }

        public IClientService<AuthClientEntity> GetAuthClient() => _authService;

        public IClientService<UploadClientEntity> GetUploadClient() => _uploadService;

        public bool IsReady() => _authService.GetEntity().Ready && _uploadService.GetEntity().Ready;

        public int GetPlayerCount() => AuthorizedGameClients.Count;

        public int Initialize()
        {
            if (ConnectToAuth() > 0 || ConnectToUpload() > 0)
                return 1;

            if (SendGSInfoToAuth() > 0 || SendInfoToUpload() > 0)
                return 1;

            return 0;
        }

        private int ConnectToAuth()
        {
            string addrStr = _networkOptions.AuthIp;
            int port = _networkOptions.AuthPort;

            if (string.IsNullOrEmpty(addrStr) || port == 0)
            {
                _notificationSvc.WriteError("Invalid network auth.io.ip configuration! Review your appsettings.json!");
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
                _authService.Create(this, authSock);
                
                status = _authService.Connect(authEp);
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

            _notificationSvc.WriteDebug(new[] { "Connected to Auth server successfully!" }, true);

            return 0;
        }

        private int SendGSInfoToAuth()
        {
            try
            {
                var index = _serverServerOptions.Index;
                var ip = _networkOptions.Ip;
                var port = _networkOptions.Port;
                var name = _serverServerOptions.Name;
                var screenshotUrl = _serverServerOptions.ScreenshotUrl;
                var isAdultServer = _serverServerOptions.IsAdultServer;

                var msg = new TS_GA_LOGIN(index, ip, port, name, screenshotUrl, isAdultServer);

                _authService.PendMessage(msg);
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
                _notificationSvc.WriteError("Invalid network io.upload.ip configuration! Review your appsettings.json!");
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
                _uploadService.Create(this, uploadSock);

                status = _uploadService.Connect(uploadEp);
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

            _notificationSvc.WriteDebug(new[] { "Connected to Upload server successfully!" }, true);

            return 0;
        }

        private int SendInfoToUpload()
        {
            try
            {
                var serverName = _serverServerOptions.Name;
                var msg = new TS_SU_LOGIN(serverName);

                _uploadService.PendMessage(msg);
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

            // TODO Debug if this is working correctly
            ClientService<GameClientEntity> client = new(_logOptions, _notificationSvc, _networkIOptions);
            client.Create(this, socket);

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

        public bool RegisterAccount(ClientService<GameClientEntity> client, string accountName)
        {
            if (AuthorizedGameClients.ContainsKey(accountName))
                return false;

            AuthorizedGameClients[accountName] = client;

            return true;
        }


    }
}
