using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Configuration;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Game.Network.Entities;
using Navislamia.Game.Services;
using Navislamia.Network.Enums;
using Navislamia.Network.Packets;
using Navislamia.Network.Packets.Actions;
using Navislamia.Network.Packets.Auth;
using Navislamia.Network.Packets.Upload;
using Navislamia.Notification;

namespace Navislamia.Game.Network
{
    public class NetworkModule : INetworkModule
    {
        private TcpListener _listener;
        private readonly IClientService<AuthClientEntity> _authService;
        private readonly IClientService<UploadClientEntity> _uploadService;

        private readonly INotificationModule _notificationSvc;
        private readonly IOptions<NetworkOptions> _networkIOptions;
        private readonly NetworkOptions _networkOptions;
        private readonly ServerOptions _serverOptions;
        private readonly IOptions<LogOptions> _logOptions;

        public Dictionary<string, ClientService<GameClientEntity>> UnauthorizedGameClients { get; set; } = new();
        public Dictionary<string,  ClientService<GameClientEntity>> AuthorizedGameClients { get; set; } = new();

        public AuthActions AuthActions { get; }
        public GameActions GameActions { get; }
        public UploadActions UploadActions { get; }

        public NetworkModule(IClientService<AuthClientEntity> authService, IClientService<UploadClientEntity> uploadService,
            IOptions<NetworkOptions> networkOptions, INotificationModule notificationModule, IOptions<LogOptions> logOptions,
            IOptions<ServerOptions> serverOptions, ICharacterService characterService) 
        {
            _notificationSvc = notificationModule;
            _authService = authService;
            _uploadService = uploadService;
            _networkIOptions = networkOptions;
            _networkOptions = networkOptions.Value;
            _serverOptions = serverOptions.Value;
            _logOptions = logOptions;

            AuthActions = new AuthActions(notificationModule, this);
            GameActions = new GameActions(notificationModule, this, _networkOptions, characterService);
            UploadActions = new UploadActions(notificationModule);
        }

        public IClientService<AuthClientEntity> GetAuthClient() => _authService;

        public IClientService<UploadClientEntity> GetUploadClient() => _uploadService;

        public bool IsReady() => _authService.GetEntity().Ready && _uploadService.GetEntity().Ready;

        public int GetPlayerCount() => AuthorizedGameClients.Count;

        public void Initialize()
        {
             ConnectToAuth();
             ConnectToUpload();
             SendGsInfoToAuth();
             SendInfoToUpload();
        }
        public void Shutdown()
        {
            _notificationSvc.WriteString("NetworkModule is shutting down...\n");

            _authService.Disconnect();
            _uploadService.Disconnect();

            using (var clientEnumerator = UnauthorizedGameClients.GetEnumerator())
            {
                while (clientEnumerator.MoveNext())
                {
                    IClientService<GameClientEntity> client = clientEnumerator.Current.Value;

                    // TODO: send logout packet to client
                    client.Disconnect();
                }

                UnauthorizedGameClients.Clear();
            }

            using (var clientEnumerator = AuthorizedGameClients.GetEnumerator())
            {
                while (clientEnumerator.MoveNext())
                {
                    IClientService<GameClientEntity> client = clientEnumerator.Current.Value;

                    // TODO: send logout packet to client
                    client.Disconnect();
                }

                AuthorizedGameClients.Clear();
            }

            _notificationSvc.WriteSuccess("NetworkModule has successfully shutdown!");
        }

        private void ConnectToAuth()
        {
            string addrStr = _networkOptions.Auth.Ip;
            int port = _networkOptions.Auth.Port;

            if (string.IsNullOrEmpty(addrStr) || port == 0)
            {
                _notificationSvc.WriteError("Invalid network auth ip! Review your configuration!");
                throw new Exception();
            }

            IPAddress addr;

            if (!IPAddress.TryParse(addrStr, out addr))
            {
                _notificationSvc.WriteError($"Failed to parse auth ip: {addrStr}");
                throw new Exception();

            }

            IPEndPoint authEp = new IPEndPoint(addr, port);
            var authSock = new Socket(addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            
            try
            {
                _authService.Create(this, authSock);
                _authService.Connect(authEp);
            }
            catch (Exception ex)
            {
                _notificationSvc.WriteException(ex);
                _notificationSvc.WriteError("Failed to connect to the auth server!");
                throw new Exception();

            }
            
            _notificationSvc.WriteDebug(new[] { "Connected to Auth server successfully!" }, true);
        }

        private void SendGsInfoToAuth()
        {
            try
            {
                var index = _serverOptions.Index;
                var name = _serverOptions.Name;
                var screenshotUrl = _serverOptions.ScreenshotUrl;
                var isAdultServer = _serverOptions.IsAdultServer;
                var ip = _networkOptions.Game.Ip;
                var port = _networkOptions.Game.Port;

                var msg = new Packet<TS_GA_LOGIN>((ushort)AuthPackets.TS_GA_LOGIN, new(index, name, screenshotUrl, (byte)isAdultServer, ip, port));
                
                _authService.SendMessage(msg);
            }
            catch (Exception ex)
            {
                _notificationSvc.WriteError("Failed to send Game server info to the Auth Server!");
                _notificationSvc.WriteException(ex);

                throw new Exception("Failed sending message to Authservice");
            }
        }

        private void ConnectToUpload()
        {
            string addrStr = _networkOptions.Upload.Ip;
            int port = _networkOptions.Upload.Port;

            if (string.IsNullOrEmpty(addrStr) || port == 0)
            {
                _notificationSvc.WriteError("Invalid network io.upload.ip configuration! Review your Configuration.json!");
            }

            IPAddress addr;

            if (!IPAddress.TryParse(addrStr, out addr))
            {
                _notificationSvc.WriteError($"Failed to parse io.upload.ip: {addrStr}");
                throw new Exception("Could not read upload ip");
            }

            IPEndPoint uploadEp = new IPEndPoint(addr, port);
            var uploadSock = new Socket(addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _uploadService.Create(this, uploadSock);
                _uploadService.Connect(uploadEp);
            }
            catch (Exception ex)
            {
                _notificationSvc.WriteException(ex);
                _notificationSvc.WriteError("Failed to connect to the upload server!");
                throw new Exception();
            }

            _notificationSvc.WriteDebug(new[] { "Connected to Upload server successfully!" });
        }

        private void SendInfoToUpload()
        {
            try
            {
                var serverName = _serverOptions.Name;

                var msg = new Packet<TS_SU_LOGIN>((ushort)UploadPackets.TS_SU_LOGIN, new(serverName));

                _uploadService.SendMessage(msg);
            }
            catch (Exception ex)
            {
                _notificationSvc.WriteException(ex);
                throw new Exception();
            }
        }

        private int StartClientListener()
        {
            string addrStr = _networkOptions.Game.Ip;
            ushort port = _networkOptions.Game.Port;
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
