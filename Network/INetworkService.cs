using Navislamia.Network.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public interface INetworkService
    {
        public bool Ready { get; }

        public AuthClient AuthClient { get; }

        public UploadClient UploadClient { get; }

        public Dictionary<string, Client> AuthAccounts { get; set; }

        public Dictionary<string, Client> GameClients { get; set; }

        public int PlayerCount { get; }

        public int Initialize();

        public int StartListener();

        public bool RegisterAccount(Client client, string accountName);
    }
}
