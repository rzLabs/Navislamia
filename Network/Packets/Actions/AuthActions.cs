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
        private readonly INotificationModule _notificationModule;
        private readonly INetworkModule _networkModule;

        Dictionary<ushort, Func<ClientService<AuthClientEntity>, ISerializablePacket, int>> actions = new();

        public AuthActions(INotificationModule notificationModule, INetworkModule networkModule)
        {
            _notificationModule = notificationModule;
            _networkModule = networkModule;

            actions[(ushort)AuthPackets.TS_AG_LOGIN_RESULT] = OnLoginResult;
            actions[(ushort)AuthPackets.TS_AG_CLIENT_LOGIN] = OnAuthClientLoginResult;
        }

        public void Execute(ClientService<AuthClientEntity> client, ISerializablePacket msg)
        {
            if (!actions.ContainsKey(msg.ID))
            {
                return;
            }

            actions[msg.ID]?.Invoke(client, msg);
        }


        private int OnLoginResult(ClientService<AuthClientEntity> client, ISerializablePacket msg)
        {
            var agLogin = msg as TS_AG_LOGIN_RESULT;

            if (agLogin == null || agLogin.Result > 0)
            {
                _notificationModule.WriteError("Failed to register to the Auth Server!");

                return 1;
            }

            client.GetEntity().Ready = true;

            _notificationModule.WriteSuccess("Successfully registered to the Auth Server!");

            return 0;
        }

        private int OnAuthClientLoginResult(ClientService<AuthClientEntity> client, ISerializablePacket msg)
        {
            var agClientLogin = msg as TS_AG_CLIENT_LOGIN;

            ClientService<GameClientEntity> gameClient = null;

            // Check if the game client connection is queued in AuthAccounts
            if (!_networkModule.UnauthorizedGameClients.ContainsKey(agClientLogin.Account.String))
            {
                _notificationModule.WriteError($"Account register failed for: {agClientLogin?.Account.String}");
                agClientLogin.Result = (ushort)ResultCode.AccessDenied;
            }
            else
            {
                gameClient = _networkModule.UnauthorizedGameClients[agClientLogin.Account.String];

                _networkModule.UnauthorizedGameClients.Remove(agClientLogin.Account.String);

                if (agClientLogin.Result == (ushort)ResultCode.Success)
                {
                    var info = gameClient.GetEntity().Info;

                    if (!_networkModule.RegisterAccount(gameClient, agClientLogin.Account.String))
                    {
                        // TODO: SendLogoutToAuth
                        info.AuthVerified = false;
                    }
                    else
                    {
                        info.AccountName = agClientLogin.Account;
                        info.AccountID = agClientLogin.AccountID;
                        info.AuthVerified = true;
                        info.PCBangMode = agClientLogin.PcBangMode;
                        info.EventCode = agClientLogin.EventCode;
                        info.Age = agClientLogin.Age;
                        info.ContinuousPlayTime = agClientLogin.ContinuousPlayTime;
                        info.ContinuousLogoutTime = agClientLogin.ContinuousLogoutTime;
                    }
                }
            }

            gameClient?.SendResult((ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH, agClientLogin.Result);
            
            return 0;
        }
    }
}
