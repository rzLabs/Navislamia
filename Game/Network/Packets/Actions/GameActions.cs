using Navislamia.Network.Enums;
using Navislamia.Notification;
using System;
using System.Collections.Generic;
using Configuration;
using Navislamia.Game.Network;
using Navislamia.Game.Network.Entities;
using Navislamia.Network.Packets.Game;
using Navislamia.Network.Packets.Auth;

namespace Navislamia.Network.Packets.Actions
{
    public class GameActions
    {
        private readonly NetworkOptions _networkOptions;
        INotificationModule _notificationModule;
        INetworkModule _networkModule;

        Dictionary<ushort, Func<ClientService<GameClientEntity>, IPacket, int>> actions = new();

        public GameActions(INotificationModule notificationModule, INetworkModule networkModule, NetworkOptions networkOptions)
        {
            _networkOptions = networkOptions;
            _notificationModule = notificationModule;
            _networkModule = networkModule;

            actions.Add((ushort)GamePackets.TM_CS_VERSION, OnVersion);
            actions.Add((ushort)GamePackets.TS_CS_REPORT, OnReport);
            actions.Add((ushort)GamePackets.TS_CS_CHARACTER_LIST, OnCharacterList);
            actions.Add((ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH, OnAccountWithAuth);
        }

        public int Execute(ClientService<GameClientEntity> client, IPacket msg)
        {
            if (!actions.ContainsKey(msg.ID))
                return 1;

            return actions[msg.ID]?.Invoke(client, msg) ?? 2;
        }

        private int OnVersion(ClientService<GameClientEntity> client, IPacket arg)
        {
            // TODO: properly implement this action

            return 0;
        }

        private int OnReport(ClientService<GameClientEntity> arg1, IPacket arg2)
        {
            // TODO: implement me

            return 0;
        }

        private int OnCharacterList(ClientService<GameClientEntity> arg1, IPacket arg2)
        {
            // TODO: implement me

            return 0;
        }

        private int OnAccountWithAuth(ClientService<GameClientEntity> client, IPacket msg)
        {
            var _msg = msg.GetDataStruct<TM_CS_ACCOUNT_WITH_AUTH>();
            var _loginInfo = new Packet<TS_GA_CLIENT_LOGIN>((ushort)AuthPackets.TS_GA_CLIENT_LOGIN, new(_msg.Account, _msg.OneTimePassword));

            var connMax = _networkOptions.MaxConnections;

            if (_networkModule.GetPlayerCount() > connMax)
            {
                client.SendResult(msg.ID, (ushort)ResultCode.LimitMax);
                return 1;
            }

            if (string.IsNullOrEmpty(client.GetEntity().Info.AccountName))
            {
                if (_networkModule.UnauthorizedGameClients.ContainsKey(_msg.Account))
                {
                    client.SendResult(msg.ID, (ushort)ResultCode.AccessDenied);
                    return 1;
                }

                _networkModule.UnauthorizedGameClients.Add(_msg.Account, client);
            }

            if (_networkModule.GetAuthClient().GetEntity().Connected)
                _networkModule.GetAuthClient().SendMessage(_loginInfo);

            return 0;
        }
    }
}
