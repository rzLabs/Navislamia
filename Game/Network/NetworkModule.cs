using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Game.Network.Entities;
using Navislamia.Game.Services;
using Navislamia.Game.Network.Interfaces;
using Navislamia.Game.Network.Packets;

namespace Navislamia.Game.Network
{
    public class NetworkModule : INetworkModule
    {
        private Socket _clientListener;

        private readonly IOptions<NetworkOptions> _networkIOptions;
        private readonly NetworkOptions _networkOptions;
        private readonly ServerOptions _serverOptions;
        private readonly IOptions<LogOptions> _logOptions;

        private readonly IClientService _clientService;

        private ILogger<NetworkModule> _logger;

        public NetworkModule(IClientService clientService, IOptions<NetworkOptions> networkOptions, ILogger<NetworkModule> logger, IOptions<LogOptions> logOptions, IOptions<ServerOptions> serverOptions, ICharacterService characterService)
        {
            _clientService = clientService;

            _networkIOptions = networkOptions;
            _networkOptions = networkOptions.Value;
            _serverOptions = serverOptions.Value;
            _logOptions = logOptions;

            _logger = logger;
        }

        public bool Start()
        {
            ConnectToAuth();
            ConnectToUpload();
            SendGsInfoToAuth();
            SendInfoToUpload();

            var maxTime = DateTime.UtcNow.AddSeconds(30);

            while (!_clientService.IsReady)
            {
                if (DateTime.UtcNow < maxTime)
                    continue;

                _logger.LogError("Network service timed out!");

                return false;
            }

            ListenForClients();

            return true;
        }

        public void Shutdown()
        {
            _logger.LogInformation("NetworkModule is shutting down...\n");

            _clientService.AuthClient.Connection.Disconnect();
            _clientService.UploadClient.Connection.Disconnect();

            using (var clientEnumerator = _clientService.UnauthorizedGameClients.GetEnumerator())
            {
                while (clientEnumerator.MoveNext())
                {
                    GameClient client = clientEnumerator.Current.Value;

                    // TODO: send logout packet to client
                    client.Connection.Disconnect();
                }

                _clientService.UnauthorizedGameClients.Clear();
            }

            using (var clientEnumerator = _clientService.AuthorizedGameClients.GetEnumerator())
            {
                while (clientEnumerator.MoveNext())
                {
                    GameClient client = clientEnumerator.Current.Value;

                    // TODO: send logout packet to client
                    client.Connection.Disconnect();
                }

                _clientService.AuthorizedGameClients.Clear();
            }

            _logger.LogInformation("NetworkModule has successfully shutdown!");
        }

        private void ConnectToAuth() // TODO: exceptions need to write the ex message and stack trace
        {
            string addrStr = _networkOptions.Auth.Ip;
            int port = _networkOptions.Auth.Port;

            if (string.IsNullOrEmpty(addrStr) || port == 0)
            {
                _logger.LogError("Invalid network auth ip! Review your configuration!");
                throw new Exception();
            }

            IPAddress addr;

            if (!IPAddress.TryParse(addrStr, out addr))
            {
                _logger.LogError($"Failed to parse auth ip: {addrStr}");
                throw new Exception();

            }

            IPEndPoint authEp = new IPEndPoint(addr, port);

            try
            {
                _clientService.CreateAuthClient(authEp);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to connect to the auth server!");
                throw new Exception();

            }

            _logger.LogDebug( "Connected to Auth server successfully!");
            
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

                _clientService.AuthClient.SendMessage(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send Game server info to the Auth Server!");

                throw new Exception("Failed sending message to Authservice");
            };
        }

        private void ConnectToUpload()
        {
            string addrStr = _networkOptions.Upload.Ip;
            int port = _networkOptions.Upload.Port;

            if (string.IsNullOrEmpty(addrStr) || port == 0)
            {
                _logger.LogError("Invalid network io.upload.ip configuration! Review your Configuration.json!");
            }

            IPAddress addr;

            if (!IPAddress.TryParse(addrStr, out addr))
            {
                _logger.LogError($"Failed to parse io.upload.ip: {addrStr}");
                throw new Exception("Could not read upload ip");
            }

            IPEndPoint uploadEp = new IPEndPoint(addr, port);

            try
            {
                _clientService.CreateUploadClient(uploadEp);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to connect to the upload server!");
                throw new Exception();
            }

            _logger.LogDebug("Connected to Upload server successfully!"); ;
        }

        private void SendInfoToUpload()
        {
            try
            {
                var serverName = _serverOptions.Name;

                var msg = new Packet<TS_SU_LOGIN>((ushort)UploadPackets.TS_SU_LOGIN, new(serverName));

                _clientService.UploadClient.SendMessage(msg);
            }
            catch (Exception ex)
            {
                // TODO: fix me!
                //_notificationSvc.WriteException(ex);
                throw new Exception();
            }
        }

        private void ListenForClients()
        {
            string _address = _networkOptions.Game.Ip;
            ushort _port = _networkOptions.Game.Port;
            int _backlog = _networkOptions.Backlog;

            IPAddress addr;

            if (!IPAddress.TryParse(_address, out addr))
            {
                _logger.LogError("Failed to parse io.ip: {address}", _address);
                return;
            }

            var _clientListenerEndPoint = new IPEndPoint(addr, _port);

            _clientListener = new(_clientListenerEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _clientListener.Bind(_clientListenerEndPoint);
            _clientListener.Listen(_backlog);

            _logger.LogInformation("Listening for clients on {address}:{port}", _address, _port);

            Task.Run(acceptClients);
        }

        private async void acceptClients()
        {
            while (true)
            {
                var _clientSocket = await _clientListener.AcceptAsync();
                _clientSocket.NoDelay = true;

                var _gameClient = _clientService.CreateGameClient(_clientSocket);

                _logger.LogDebug("Game client connected {clientTag}", _gameClient.ClientTag);
            }
        }

    }
}
