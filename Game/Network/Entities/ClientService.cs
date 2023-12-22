using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

using Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Game.Network.Packets.Auth;
using Navislamia.Game.Services;
using Navislamia.Network.Enums;
using Navislamia.Network.Packets;
using Navislamia.Network.Packets.Actions;
using Navislamia.Network.Packets.Auth;
using Navislamia.Network.Packets.Game;
using Navislamia.Network.Packets.Upload;

using static Navislamia.Network.NetworkExtensions;
using static Navislamia.Network.Packets.PacketExtensions;

namespace Navislamia.Game.Network.Entities
{
    // TODO: Poll connections by configuration interval
    // TODO: Disconnect/Destroy

    public class ClientService : IClientService
    {
        private readonly ILogger<ClientService> _logger;

        private NetworkOptions _networkOptions;

        private readonly AuthActionService _authActionService;
        private readonly UploadActionService  _uploadActionService;
        private readonly GameActionService _gameActionService;

        public AuthClientService AuthClient { get; private set; }

        public UploadClientService UploadClient { get; private set; }

        public bool AuthReady { get; set; } = false;

        public bool UploadReady { get; set; } = false; 

        public bool IsReady
        {
            get
            {
                return AuthReady && UploadReady;
            }
        }

        public Dictionary<string, GameClientService> UnauthorizedGameClients { get; set; } = new Dictionary<string, GameClientService>();

        public Dictionary<string, GameClientService> AuthorizedGameClients { get; set; } = new Dictionary<string, GameClientService>();

        public ClientService(ILogger<ClientService> logger, IOptions<NetworkOptions> networkOptions, ICharacterService characterService)
        {
            _logger = logger;

            _networkOptions = networkOptions.Value;

            _authActionService = new AuthActionService(this);
            _uploadActionService = new UploadActionService(this);
            _gameActionService = new GameActionService(this, _networkOptions, characterService);
        }

        public AuthClientService CreateAuthClient(IPEndPoint authEndPoint)
        {
            var _authSock = new Socket(authEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _authSock.Connect(authEndPoint.Address, authEndPoint.Port);

            AuthClient = new AuthClientService(_authSock, _authActionService);
            AuthClient.Start();

            return AuthClient;
        }

        public UploadClientService CreateUploadClient(IPEndPoint uploadEndPoint)
        {
            var _uploadSocket = new Socket(uploadEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _uploadSocket.Connect(uploadEndPoint.Address, uploadEndPoint.Port);

            UploadClient = new UploadClientService(_uploadSocket, _uploadActionService);
            UploadClient.Start();

            return UploadClient;
        }

        public GameClientService CreateGameClient(Socket socket)
        {
            var _gameClient = new GameClientService(socket, _networkOptions.CipherKey, _gameActionService);
            _gameClient.Start();

            return _gameClient;
        }

        public bool RegisterGameClient(string account, GameClientService clientService)
        {
            if (AuthorizedGameClients.ContainsKey(account))
                return false;

            AuthorizedGameClients[account] = clientService;

            return true;
        }

        public void RemoveGameClient(string account, GameClientService client)
        {
            var _clientInfo = client.Info;
            var _msg = new Packet<TS_GA_CLIENT_LOGOUT>((ushort)AuthPackets.TS_GA_CLIENT_LOGOUT, new(_clientInfo.AccountName, (uint)_clientInfo.ContinuousPlayTime));

            AuthClient.SendMessage(_msg);

            AuthorizedGameClients.Remove(_clientInfo.AccountName);

            _logger.LogDebug($"{client.ClientTag} disconnected!");


            AuthorizedGameClients.Remove(account);
        }

        public int ClientCount
        {
            get
            {
                return AuthorizedGameClients.Count;
            }
        }
    }      
}



