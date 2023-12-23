using System;
using System.IO;
using Microsoft.Extensions.Options;

using Navislamia.Configuration.Options;
using Navislamia.Game.Models.Navislamia;
using Navislamia.Game.Network;
using Navislamia.Game.Repositories;
using Navislamia.Game.Maps;
using Navislamia.Game.Scripting;
using Navislamia.Game.Services;
using Microsoft.Extensions.Logging;
using Configuration;
using Navislamia.Game.Network.Packets;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using Navislamia.Game.Network.Interfaces;

namespace Navislamia.Game
{
    public class GameModule : IGameModule
    {
        private Socket _clientListener;

        private readonly IClientService _clientService;
        private readonly IScriptService _scriptService;
        private readonly IMapService _mapService;
        private readonly ScriptOptions _scriptOptions;
        private readonly MapOptions _mapOptions;
        private readonly ServerOptions _serverOptions;
        private readonly NetworkOptions _networkOptions;

        private readonly IWorldRepository _worldRepository;
        private readonly ICharacterService _characterService;
        private readonly WorldEntity _worldEntity;

        private readonly ILogger<GameModule> _logger;

        public GameModule(IOptions<ScriptOptions> scriptOptions, IOptions<MapOptions> mapOptions,  IOptions<ServerOptions> serverOptions, IOptions<NetworkOptions> networkOptions,
            ILogger<GameModule> logger,
            IClientService clientService, IScriptService scriptService, IMapService mapService, IWorldRepository worldRepository, ICharacterService characterService)
        {
            _scriptOptions = scriptOptions.Value;
            _mapOptions = mapOptions.Value;
            _serverOptions = serverOptions.Value;
            _networkOptions = networkOptions.Value;
            _worldRepository = worldRepository;
            _characterService = characterService;

            _logger = logger;

            _worldEntity = worldRepository.LoadWorldIntoMemory();

            _clientService = clientService;
            _scriptService = scriptService;
            _mapService = mapService;
        }

        public void Start(string ip, int port, int backlog)
        {   
            if (!LoadScripts(_scriptOptions.SkipLoading))
                return;

            if (!LoadMaps(_mapOptions.SkipLoading))
                return;

            if (!StartNetwork())
                return;

        }

        private bool LoadMaps(bool skip)
        {
            if (skip)
            {
                _logger.LogWarning("Map loading disabled!");
                return true;
            }

            // TODO: MapContent should be printing messages
            return _mapService.Start($"{Directory.GetCurrentDirectory()}\\Maps");
        }

        private bool LoadScripts(bool skip)
        {
            if (skip)
            {
                _logger.LogWarning("Script loading disabled!");
                return true;
            }

            return _scriptService.Start();
        }

        private bool StartNetwork()
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

            _logger.LogDebug("Connected to Auth server successfully!");

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
