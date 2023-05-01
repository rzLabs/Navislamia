using Navislamia.Network.Enums;
using Navislamia.Notification;
using System;
using System.Collections.Generic;
using Navislamia.Network.Entities;
using Navislamia.Network.Packets.Auth;

namespace Navislamia.Network.Packets.Actions
{
    public class AuthActions
    {
        private readonly INotificationService _notificationService;
        private readonly INetworkModule _networkModule;

        Dictionary<ushort, Func<ClientService<AuthClientEntity>, ISerializablePacket, int>> actions = new();

        public AuthActions(INotificationService notificationService, INetworkModule networkModule)
        {
            _notificationService = notificationService;
            _networkModule = networkModule;

            actions[(ushort)AuthPackets.TS_AG_LOGIN_RESULT] = OnLoginResult;
            actions[(ushort)AuthPackets.TS_AG_CLIENT_LOGIN] = OnAuthClientLoginResult;
        }

        public int Execute(ClientService<AuthClientEntity> client, ISerializablePacket msg)
        {
            if (!actions.ContainsKey(msg.ID))
                return 1;

            return actions[msg.ID]?.Invoke(client, msg) ?? 2;
        }


        private int OnLoginResult(ClientService<AuthClientEntity> client, ISerializablePacket msg)
        {
            var _msg = msg as TS_AG_LOGIN_RESULT;

            if (_msg.Result > 0)
            {
                _notificationService.WriteError("Failed to register to the Auth Server!");

                return 1;
            }

            client.GetEntity().Ready = true;

            _notificationService.WriteSuccess("Successfully registered to the Auth Server!");

            return 0;
        }

        private int OnAuthClientLoginResult(ClientService<AuthClientEntity> client, ISerializablePacket msg)
        {
            var _msg = msg as TS_AG_CLIENT_LOGIN;

            ClientService<GameClientEntity> gameClient = null;

            // Check if the game client connection is queued in AuthAccounts
            if (!_networkModule.UnauthorizedGameClients.ContainsKey(_msg.Account.String))
            {
                _notificationService.WriteError($"Account register failed for: {_msg.Account.String}");

                _msg.Result = (ushort)ResultCode.AccessDenied;
            }
            else
            {
                gameClient = _networkModule.UnauthorizedGameClients[_msg.Account.String];

                _networkModule.UnauthorizedGameClients.Remove(_msg.Account.String);

                if (_msg.Result == (ushort)ResultCode.Success)
                {
                    var info = gameClient.GetEntity().Info;

                    if (!_networkModule.RegisterAccount(gameClient, _msg.Account.String))
                    {
                        // TODO: SendLogoutToAuth
                        info.AuthVerified = false;
                    }
                    else
                    {
                        info.AccountName = _msg.Account;
                        info.AccountID = _msg.AccountID;
                        info.AuthVerified = true;
                        info.PCBangMode = _msg.PcBangMode;
                        info.EventCode = _msg.EventCode;
                        info.Age = _msg.Age;
                        info.ContinuousPlayTime = _msg.ContinuousPlayTime;
                        info.ContinuousLogoutTime = _msg.ContinuousLogoutTime;
                    }
                }
            }

            gameClient?.SendResult((ushort)ClientPackets.TM_CS_ACCOUNT_WITH_AUTH, _msg.Result);
            
            return 0;
        }
    }
}
