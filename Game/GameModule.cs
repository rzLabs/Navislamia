using System;
using System.IO;
using Microsoft.Extensions.Options;

using Navislamia.Configuration.Options;
using Navislamia.Game.Network;
using Navislamia.Game.Maps;
using Navislamia.Game.Scripting;
using Navislamia.Game.Services;
using Microsoft.Extensions.Logging;
using Navislamia.Game.Network.Packets;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using Navislamia.Game.DataAccess.Entities.Navislamia;
using Navislamia.Game.DataAccess.Repositories.Interfaces;
using Navislamia.Game.Network.Interfaces;
using Navislamia.Game.Network.Packets.Auth;
using Navislamia.Game.Network.Packets.Enums;
using Navislamia.Game.Network.Packets.Upload;

namespace Navislamia.Game;

public class GameModule : IGameModule
{
    private Socket _clientListener;

    private readonly INetworkService _networkService;
    private readonly IScriptService _scriptService;
    private readonly GameOptions _gameOptions;
    private readonly IMapService _mapService;
    private readonly ScriptOptions _scriptOptions;
    private readonly MapOptions _mapOptions;
    private readonly ServerOptions _serverOptions;
    private readonly NetworkOptions _networkOptions;

    private readonly IWorldRepository _worldRepository;
    private readonly ICharacterService _characterService;
    private readonly WorldEntity _worldEntity;

    private readonly ILogger<GameModule> _logger;

    public GameModule(IOptions<ScriptOptions> scriptOptions, IOptions<MapOptions> mapOptions, 
        IOptions<ServerOptions> serverOptions, IOptions<NetworkOptions> networkOptions, IOptions<GameOptions> gameOptions, ILogger<GameModule> logger,
        INetworkService networkService, IScriptService scriptService, IMapService mapService, 
        IWorldRepository worldRepository, ICharacterService characterService)
    {
        _gameOptions = gameOptions.Value;
        _scriptOptions = scriptOptions.Value;
        _mapOptions = mapOptions.Value;
        _serverOptions = serverOptions.Value;
        _networkOptions = networkOptions.Value;
        _worldRepository = worldRepository;
        _characterService = characterService;

        _logger = logger;

        _worldEntity = worldRepository.LoadWorldIntoMemory();

        _networkService = networkService;
        _scriptService = scriptService;
        _mapService = mapService;
    }

    public void Start()
    {
        LoadScripts();
        LoadMaps();
        StartNetwork();
    }

    private void LoadMaps()
    {
        if (!_mapOptions.SkipLoading)
        {
            // TODO: MapContent should be printing messages
            _mapService.Start($"{Directory.GetCurrentDirectory()}\\Maps");

            return;
        }
        
        _logger.LogWarning("Map loading disabled!");
    }

    private void LoadScripts()
    {
        if (!_scriptOptions.SkipLoading)
        {
            _scriptService.Start();

            return;
        }
        
        _logger.LogWarning("Script loading disabled!");
    }

    private void StartNetwork()
    {
        ConnectToAuth();
        ConnectToUpload();
        SendGsInfoToAuth();
        SendInfoToUpload();

        var maxTime = DateTime.UtcNow.AddSeconds(30);

        while (!_networkService.IsReady())
        {
            if (DateTime.UtcNow < maxTime)
            {
                continue;
            }

            _logger.LogError("Network service timed out!");

            return;
        }

        ListenForClients();
    }

    private void ConnectToAuth() // TODO: exceptions need to write the ex message and stack trace
    {
        try
        {
            _networkService.CreateAuthClient();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to connect to the auth server! {exception}", ex);
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
            _networkService.SendMessageToAuth(msg);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to send Game server info to the Auth Server! {exception}", ex);
            throw new Exception("Failed sending message to Authservice");
        };
    }

    private void ConnectToUpload()
    {
        try
        {
            _networkService.CreateUploadClient();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to connect to the upload server! {exception}", ex);
            throw new Exception();
        }

        _logger.LogDebug("Connected to Upload server successfully!"); ;
    }

    private void SendInfoToUpload()
    {
        try
        {
            var serverName = _serverOptions.Name;

            var msg = new Packet<TS_SU_LOGIN>((ushort)UploadPackets.TS_SU_LOGIN, new TS_SU_LOGIN(serverName));

            _networkService.SendMessageToUpload(msg);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to send Game server info to the Upload Server! {exception}", ex);
            throw new Exception("Failed sending message to Upload Server");
        }
    }

    private void ListenForClients()
    {
        var address = _networkOptions.Game.Ip;
        var port = _networkOptions.Game.Port;
        var backlog = _networkOptions.Backlog;

        if (!IPAddress.TryParse(address, out var addr))
        {
            _logger.LogError("Failed to parse IP: {address}", address);
            return;
        }

        var clientListenerEndPoint = new IPEndPoint(addr, port);
        _clientListener = new Socket(clientListenerEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _clientListener.Bind(clientListenerEndPoint);
        _clientListener.Listen(backlog);

        _logger.LogInformation("Listening for clients on {address}:{port}", address, port);

        Task.Run(AcceptClients);
    }

    private async void AcceptClients()
    {
        while (true)
        {
            var clientSocket = await _clientListener.AcceptAsync();
            var client = _networkService.CreateGameClient(clientSocket, _gameOptions);
            
            clientSocket.NoDelay = true;
    
            _logger.LogDebug("Client connected {clientTag}", client.ClientTag);
        }
    }
}