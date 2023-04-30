using Configuration;
using Navislamia.Network.Enums;
using Network;
using Navislamia.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Navislamia.Network.Packets.Auth;
using Navislamia.Network.Packets;
using Navislamia.Network.Entities;
using Navislamia.Network.Packets.Actions.Interfaces;
using Navislamia.Network.Interfaces;

namespace Navislamia.Network.Packets.Actions
{
    public class AuthActions : IAuthActionService
    {
        INotificationService notificationSVC;
        INetworkService networkSVC;

        Dictionary<ushort, Func<IClient, ISerializablePacket, int>> actions = new();

        public AuthActions(INotificationService notificationService, INetworkService networkService)
        {
            notificationSVC = notificationService;
            networkSVC = networkService;

            actions[(ushort)AuthPackets.TS_AG_LOGIN_RESULT] = OnLoginResult;
            actions[(ushort)AuthPackets.TS_AG_CLIENT_LOGIN] = OnAuthClientLoginResult;
        }

        public int Execute(IClient client, ISerializablePacket msg)
        {
            if (!actions.ContainsKey(msg.ID))
                return 1;

            return actions[msg.ID]?.Invoke(client, msg) ?? 2;
        }

        public int OnLoginResult(IClient client, ISerializablePacket msg)
        {
            var _msg = msg as TS_AG_LOGIN_RESULT;

            if (_msg.Result > 0)
            {
                notificationSVC.WriteError("Failed to register to the Auth Server!");

                return 1;
            }

            ((AuthClient)client).GetEntity().Ready = true;

            notificationSVC.WriteSuccess("Successfully registered to the Auth Server!");

            return 0;
        }

        public int OnAuthClientLoginResult(IClient client, ISerializablePacket msg)
        {
            var _msg = msg as TS_AG_CLIENT_LOGIN;

            GameClient gameClient = null;

            // Check if the game client connection is queued in AuthAccounts
            if (!networkSVC.AuthAccounts.ContainsKey(_msg.Account.String))
            {
                notificationSVC.WriteError($"Account register failed for: {_msg.Account.String}");

                _msg.Result = (ushort)ResultCode.AccessDenied;
            }
            else
            {
                gameClient = networkSVC.AuthAccounts[_msg.Account.String] as GameClient;

                networkSVC.AuthAccounts.Remove(_msg.Account.String);

                if (_msg.Result == (ushort)ResultCode.Success)
                {
                    var info = gameClient.GetEntity()?.Info;

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

            gameClient?.SendResult((ushort)ClientPackets.TM_CS_ACCOUNT_WITH_AUTH, _msg.Result);
            
            return 0;
        }
    }
}
