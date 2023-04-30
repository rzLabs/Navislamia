using Navislamia.Network.Entities;
using System;
using System.Collections.Generic;
using System.Text;

using Navislamia.Network.Interfaces;

namespace Network
{
    public interface INetworkService
    {
        public bool Ready { get; }

        public Dictionary<string, ClientService<GameClientEntity>> UnauthorizedGameClients { get; set; }

        public Dictionary<string, ClientService<GameClientEntity>> AuthorizedGameClients { get; set; }

        public int PlayerCount { get; }

        public int Initialize();

        public int StartListener();

        public bool RegisterAccount(ClientService<GameClientEntity> client, string accountName);

        public IClientService<AuthClientEntity> GetAuthClient();
        
        public IClientService<UploadClientEntity> GetUploadClient();
    }
}
