using Configuration;
using Navislamia.Network.Enums;
using Network;
using Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Navislamia.Network.Packets.Auth;
using Navislamia.Network.Packets;
using Navislamia.Network.Entities;
using Navislamia.Network.Packets.Actions.Interfaces;

namespace Navislamia.Network.Packets.Actions
{
    public class AuthActions : IAuthActionService
    {
        IConfigurationService configSVC;
        INotificationService notificationSVC;
        INetworkService networkSVC;

        Dictionary<ushort, Func<Client, ISerializablePacket, int>> actions = new Dictionary<ushort, Func<Client, ISerializablePacket, int>>();

        public AuthActions(IConfigurationService configService, INotificationService notificationService, INetworkService networkService)
        {
            configSVC = configService;
            notificationSVC = notificationService;
            networkSVC = networkService;

            actions[(ushort)AuthPackets.TS_AG_LOGIN_RESULT] = OnLoginResult;
            actions[(ushort)AuthPackets.TS_AG_CLIENT_LOGIN] = OnAuthClientLoginResult;
        }

        public int Execute(Client client, ISerializablePacket msg)
        {
            if (!actions.ContainsKey(msg.ID))
                return 1;

            return actions[msg.ID]?.Invoke(client, msg) ?? 2;
        }

        public int OnLoginResult(Client client, ISerializablePacket msg)
        {
            var _msg = msg as TS_AG_LOGIN_RESULT;

            if (_msg.Result > 0)
            {
                notificationSVC.WriteError("Failed to register to the Auth Server!");

                return 1;
            }

            client.Ready = true;

            notificationSVC.WriteSuccess("Successfully registered to the Auth Server!");

            return 0;
        }

        public int OnAuthClientLoginResult(Client client, ISerializablePacket msg)
        {
            var _msg = msg as TS_AG_CLIENT_LOGIN;
            GameClient _gameClient = null;

            // Check if the game client connection is queued in AuthAccounts
            if (!networkSVC.AuthAccounts.ContainsKey(_msg.Account.String))
            {
                notificationSVC.WriteError($"Account register failed for: {_msg.Account.String}");

                _msg.Result = (ushort)ResultCode.AccessDenied;
            }
            else
            {
                _gameClient = networkSVC.AuthAccounts[_msg.Account.String] as GameClient;

                networkSVC.AuthAccounts.Remove(_msg.Account.String);

                if (_msg.Result == (ushort)ResultCode.Success)
                {
                    var info = _gameClient.ClientInfo;

                    if (!networkSVC.RegisterAccount(client, _msg.Account.String))
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

            _gameClient.SendResult((ushort)ClientPackets.TM_CS_ACCOUNT_WITH_AUTH, _msg.Result);
            
            return 0;
        }
    }
}
