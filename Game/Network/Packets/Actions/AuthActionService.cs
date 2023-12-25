using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Navislamia.Game.Network.Interfaces;

namespace Navislamia.Game.Network.Packets.Actions;

public class AuthActionService : IAuthActionService
{
    private readonly ILogger<AuthActionService> _logger;
    private readonly Dictionary<ushort, Action<AuthClientService, IPacket>> _actions = new();
    private readonly IGameClientService _gameClientService;

    public AuthActionService(ILogger<AuthActionService> logger, IGameClientService gameClientService)
    {
        _logger = logger;
        _gameClientService = gameClientService;

        _actions[(ushort)AuthPackets.TS_AG_LOGIN_RESULT] = OnLoginResult;
        _actions[(ushort)AuthPackets.TS_AG_CLIENT_LOGIN] = OnAuthClientLoginResult;
    }

    public void Execute(AuthClientService clientService, IPacket packet)
    {
        if (!_actions.TryGetValue(packet.ID, out var action))
        {
            return;
        }

        action?.Invoke(clientService, packet);
    }

    private void OnLoginResult(AuthClientService clientService, IPacket packet)
    {
        var msg = packet.GetDataStruct<TS_AG_LOGIN_RESULT>();

        if (msg.Result > 0)
        {
            _logger.LogError("Failed to register to the Auth Server!");
            throw new Exception();
        }
        
        _logger.LogDebug("Successfully registered to the Auth Server!");
    }

    private void OnAuthClientLoginResult(AuthClientService clientService, IPacket packet)
    {
        var agClientLogin = packet.GetDataStruct<TS_AG_CLIENT_LOGIN>();
        var client = _gameClientService.GetUnauthorizedClients().FirstOrDefault(g => g.AccountName == agClientLogin.Account);
        
        // Check if the game networkService connection is queued in AuthAccounts
        if (_gameClientService.IsAuthorized(agClientLogin.Account))
        {
            _logger.LogError("Account register failed for: {accountName}", agClientLogin.Account);
            agClientLogin.Result = (ushort)ResultCode.AccessDenied;
        }
        else
        {
            if (client == null)
            {
                throw new Exception("Could not find client on login result");
            }
            
            if (agClientLogin.Result == (ushort)ResultCode.Success)
            {
                // user is already islogged in, send logout to auth
                if (_gameClientService.IsAuthorized(agClientLogin.Account))
                {
                    // TODO: SendLogoutToAuth
                    client.ConnectionData.AuthVerified = false;
                }
                else
                {
                    client.ConnectionData.AccountName = agClientLogin.Account;
                    client.ConnectionData.AccountID = agClientLogin.AccountID;
                    client.ConnectionData.AuthVerified = true;
                    client.ConnectionData.PCBangMode = agClientLogin.PcBangMode;
                    client.ConnectionData.EventCode = agClientLogin.EventCode;
                    client.ConnectionData.Age = agClientLogin.Age;
                    client.ConnectionData.ContinuousPlayTime = agClientLogin.ContinuousPlayTime;
                    client.ConnectionData.ContinuousLogoutTime = agClientLogin.ContinuousLogoutTime;
                }
            }
            _gameClientService.AuthorizeClient(agClientLogin.Account, client);
        }

        _gameClientService.SendResult(client, (ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH, agClientLogin.Result);
    }
}