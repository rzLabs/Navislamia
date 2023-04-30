using System.Collections.Generic;
using Navislamia.Network.Entities;
using Navislamia.Network.Interfaces;

namespace Navislamia.Network
{
    public interface INetworkService
    {
        public bool Ready { get; }

        public AuthClient AuthClient { get; }

        public UploadClient UploadClient { get; }

        public Dictionary<string, IClient> AuthAccounts { get; set; }

        public Dictionary<string, IClient> GameClients { get; set; }

        public int PlayerCount { get; }

        public void Initialize();

        public int StartListener();

        public bool RegisterAccount(IClient client, string accountName);
    }
}
