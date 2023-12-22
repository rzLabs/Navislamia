using System.Collections.Generic;
using Navislamia.Game.Network.Entities;
using Navislamia.Game.Network.Enums;
using Navislamia.Network.Packets;
using Navislamia.Network.Packets.Actions;

namespace Navislamia.Game.Network
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

        void SetReadiness(NetworkReadiness readinessFlag);

        bool IsReady { get; }

        int GetPlayerCount();

        bool Initialize();

        void StartListener();

        bool RegisterAccount(ClientService<GameClientEntity> client, string accountName);

        void RemoveGameClient(ClientService<GameClientEntity> client);

    }
}
