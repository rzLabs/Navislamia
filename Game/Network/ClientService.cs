using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

using Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Navislamia.Game.Services;
using Navislamia.Game.Network.Interfaces;
using Navislamia.Game.Network.Packets;


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

        public AuthClient AuthClient { get; private set; }

        public UploadClient UploadClient { get; private set; }

        public bool AuthReady { get; set; } = false;

        public bool UploadReady { get; set; } = false; 

        public bool IsReady
        {
            get
            {
                return AuthReady && UploadReady;
            }
        }

        public Dictionary<string, GameClient> UnauthorizedGameClients { get; set; } = new Dictionary<string, GameClient>();

        public Dictionary<string, GameClient> AuthorizedGameClients { get; set; } = new Dictionary<string, GameClient>();

        public ClientService(ILogger<ClientService> logger, IOptions<NetworkOptions> networkOptions, ICharacterService characterService)
        {
            _logger = logger;

            _networkOptions = networkOptions.Value;

            _authActionService = new AuthActionService(this);
            _uploadActionService = new UploadActionService(this);
            _gameActionService = new GameActionService(this, _networkOptions, characterService);
        }

        public AuthClient CreateAuthClient(IPEndPoint authEndPoint)
        {
            var _authSock = new Socket(authEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _authSock.Connect(authEndPoint.Address, authEndPoint.Port);

            AuthClient = new AuthClient(_authSock, _authActionService);
            AuthClient.Start();

            return AuthClient;
        }

        public UploadClient CreateUploadClient(IPEndPoint uploadEndPoint)
        {
            var _uploadSocket = new Socket(uploadEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _uploadSocket.Connect(uploadEndPoint.Address, uploadEndPoint.Port);

            UploadClient = new UploadClient(_uploadSocket, _uploadActionService);
            UploadClient.Start();

            return UploadClient;
        }

        public GameClient CreateGameClient(Socket socket)
        {
            var _gameClient = new GameClient(socket, _networkOptions.CipherKey, _gameActionService);
            _gameClient.Start();

            return _gameClient;
        }

        public bool RegisterGameClient(string account, GameClient clientService)
        {
            if (AuthorizedGameClients.ContainsKey(account))
                return false;

            AuthorizedGameClients[account] = clientService;

            return true;
        }

        public void RemoveGameClient(string account, GameClient client)
        {
            var _clientInfo = client.Info;
            var _msg = new Packet<TS_GA_CLIENT_LOGOUT>((ushort)AuthPackets.TS_GA_CLIENT_LOGOUT, new(_clientInfo.AccountName, (uint)_clientInfo.ContinuousPlayTime));

            AuthClient.SendMessage(_msg);

            AuthorizedGameClients.Remove(_clientInfo.AccountName);

            _logger.LogDebug("{clientTag} disconnected!", client.ClientTag);


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



