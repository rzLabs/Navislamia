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

        public AuthClient AuthClient { get; }

        public UploadClient UploadClient { get; }

        public Dictionary<string, IClient> AuthAccounts { get; set; }

        public Dictionary<string, IClient> GameClients { get; set; }

        public int PlayerCount { get; }

        public int Initialize();

        public int StartListener();

        public bool RegisterAccount(IClient client, string accountName);
    }
}
