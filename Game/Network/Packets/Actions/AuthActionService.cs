using Navislamia.Network.Enums;
using System;
using System.Collections.Generic;
using Navislamia.Game.Network;
using Navislamia.Game.Network.Entities;
using Navislamia.Network.Packets.Auth;

using Navislamia.Game.Network.Enums;

using Serilog;

namespace Navislamia.Network.Packets.Actions
{
    public class AuthActionService : IAuthActionService
    {
        private readonly IClientService _clientService;
        private readonly ILogger _logger = Log.ForContext<AuthActionService>();

        Dictionary<ushort, Func<AuthClientService, IPacket, int>> actions = new();

        public AuthActionService(IClientService clientService)
        {
            _clientService = clientService;

            actions[(ushort)AuthPackets.TS_AG_LOGIN_RESULT] = OnLoginResult;
            actions[(ushort)AuthPackets.TS_AG_CLIENT_LOGIN] = OnAuthClientLoginResult;
        }

        public void Execute(AuthClientService client, IPacket msg)
        {
            if (!actions.ContainsKey(msg.ID))
            {
                return;
            }

            actions[msg.ID]?.Invoke(client, msg);
        }


        private int OnLoginResult(AuthClientService client, IPacket msg)
        {
            var agLogin = msg.GetDataStruct<TS_AG_LOGIN_RESULT>();

            if (agLogin.Result > 0)
            {
                _logger.Error("Failed to register to the Auth Server!");

                return 1;
            }

            _clientService.AuthReady = true;

            _logger.Debug("Successfully registered to the Auth Server!");

            return 0;
        }

        private int OnAuthClientLoginResult(AuthClientService client, IPacket msg)
        {
            var agClientLogin = msg.GetDataStruct<TS_AG_CLIENT_LOGIN>();

            GameClientService gameClient = null;

            // Check if the game client connection is queued in AuthAccounts
            if (!_clientService.UnauthorizedGameClients.ContainsKey(agClientLogin.Account))
            {
                _logger.Error("Account register failed for: {accountName}", agClientLogin.Account);
                agClientLogin.Result = (ushort)ResultCode.AccessDenied;
            }
            else
            {
                gameClient = _clientService.UnauthorizedGameClients[agClientLogin.Account];

                _clientService.UnauthorizedGameClients.Remove(agClientLogin.Account);

                if (agClientLogin.Result == (ushort)ResultCode.Success)
                {
                    if (!_clientService.RegisterGameClient(agClientLogin.Account, gameClient))
                    {
                        // TODO: SendLogoutToAuth
                        gameClient.Info.AuthVerified = false;
                    }
                    else
                    {
                        gameClient.Info.AccountName = agClientLogin.Account;
                        gameClient.Info.AccountID = agClientLogin.AccountID;
                        gameClient.Info.AuthVerified = true;
                        gameClient.Info.PCBangMode = agClientLogin.PcBangMode;
                        gameClient.Info.EventCode = agClientLogin.EventCode;
                        gameClient.Info.Age = agClientLogin.Age;
                        gameClient.Info.ContinuousPlayTime = agClientLogin.ContinuousPlayTime;
                        gameClient.Info.ContinuousLogoutTime = agClientLogin.ContinuousLogoutTime;
                    }
                }
            }

            if (gameClient is not null)
            {
                gameClient.SendResult((ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH, agClientLogin.Result);              
            }

            return 0;
        }
    }
}
