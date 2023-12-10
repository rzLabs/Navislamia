using System.Collections.Generic;
using Navislamia.Network.Entities;
using Navislamia.Network.Packets.Actions;

namespace Navislamia.Network
{
    public interface INetworkModule
    {
        /// <summary>
        /// Game clients that have not been authorized yet
        /// </summary>
        Dictionary<string, ClientService<GameClientEntity>> UnauthorizedGameClients { get; set; }

        /// <summary>
        /// Game clients that have been authorized and are now only the gameserver
        /// </summary>
        Dictionary<string, ClientService<GameClientEntity>> AuthorizedGameClients { get; set; }

        IClientService<AuthClientEntity> GetAuthClient();

        IClientService<UploadClientEntity> GetUploadClient();

        AuthActions AuthActions { get; }

        GameActions GameActions { get; }

        UploadActions UploadActions { get; }

        bool IsReady();

        int GetPlayerCount();

        void Initialize();

        int StartListener();

        bool RegisterAccount(ClientService<GameClientEntity> client, string accountName);

    }
}
