using System;
using System.Collections.Generic;
using Navislamia.Game.Network.Entities;
using Navislamia.Game.Network.Interfaces;
using Navislamia.Game.Network.Packets.Auth;

namespace Navislamia.Game.Network.Packets.Actions;

public class AuthActionService : IAuthActionService
{
    // private readonly ILogger<AuthActionService> _logger;
    private readonly Dictionary<ushort, Action<AuthClient, IPacket>> _actions = new();

    private readonly GameActionService _gameActions;

    public AuthActionService()
    {
        // _logger = logger;
        _actions[(ushort)AuthPackets.TS_AG_LOGIN_RESULT] = OnLoginResult;
        _actions[(ushort)AuthPackets.TS_AG_CLIENT_LOGIN] = OnAuthClientLoginResult;
    }

    public void Execute(AuthClient client, IPacket packet)
    {
        if (!_actions.TryGetValue(packet.ID, out var action))
        {
            return;
        }

        action?.Invoke(client, packet);
    }

    private void OnLoginResult(AuthClient client, IPacket packet)
    {
        var msg = packet.GetDataStruct<TS_AG_LOGIN_RESULT>();

        if (msg.Result > 0)
        {
            throw new Exception("Failed to register to the Auth Server!");
        }
        
        Console.WriteLine("Successfully registered to the Auth Server!");
        // _logger.LogDebug("Successfully registered to the Auth Server!");
    }

    private void OnAuthClientLoginResult(AuthClient client, IPacket packet)
    {
        var agClientLogin = packet.GetDataStruct<TS_AG_CLIENT_LOGIN>();
        // Check if the game networkService connection is queued in AuthAccounts
        if (gameClient.IsAuthorized)
        {
            // _logger.LogError("Account register failed for: {accountName}", agClientLogin.Account);
            agClientLogin.Result = (ushort)ResultCode.AccessDenied;
        }
        else
        {
           
            if (agClientLogin.Result == (ushort)ResultCode.Success)
            {
                // user is already islogged in, send logout to auth
                if (gameClient.IsAuthorized)
                {
                    // TODO: SendLogoutToAuth
                    gameClient.IsAuthorized = false;
                }
                else
                {
                    gameClient.AccountName = agClientLogin.Account;
                    gameClient.AccountId = agClientLogin.AccountID;
                    gameClient.AuthVerified = true;
                    gameClient.PcBangMode = agClientLogin.PcBangMode;
                    gameClient.EventCode = agClientLogin.EventCode;
                    gameClient.Age = agClientLogin.Age;
                    gameClient.ContinuousPlayTime = agClientLogin.ContinuousPlayTime;
                    gameClient.ContinuousLogoutTime = agClientLogin.ContinuousLogoutTime;
                }
            }

            gameClient.IsAuthorized = true;
        }

        gameClient.SendResult((ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH, agClientLogin.Result);
    }
}