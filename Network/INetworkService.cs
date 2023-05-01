using Navislamia.Network.Entities;
using System.Collections.Generic;
using Navislamia.Network.Packets.Actions;

namespace Network
{
    public interface INetworkService
    {
        public Dictionary<string, ClientService<GameClientEntity>> UnauthorizedGameClients { get; set; }

        public Dictionary<string, ClientService<GameClientEntity>> AuthorizedGameClients { get; set; }

        public IClientService<AuthClientEntity> AuthClient { get; }

        public IClientService<UploadClientEntity> UploadClient { get; }

        public AuthActions AuthActions { get; }

        public GameActions GameActions { get; }

        public UploadActions UploadActions { get; }

        public bool Ready { get; }


        public int PlayerCount { get; }

        public int Initialize();

        public int StartListener();

        public bool RegisterAccount(ClientService<GameClientEntity> client, string accountName);


    }
}
