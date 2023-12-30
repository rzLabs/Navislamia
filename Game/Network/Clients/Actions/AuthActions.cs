using System;
using System.Collections.Generic;
using Navislamia.Game.Network.Interfaces;
using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Packets.Auth;
using Navislamia.Game.Network.Packets.Enums;
using Navislamia.Game.Network.Packets.Interfaces;
using Serilog;

namespace Navislamia.Game.Network.Clients.Actions;

public class AuthActions : IActions
{
    private readonly ILogger _logger = Log.ForContext<AuthActions>();
    private readonly Dictionary<ushort, Action<AuthClient, IPacket>> _actions = new();

    private readonly NetworkService _networkService;

    public AuthActions(NetworkService networkService)
    {
        _networkService = networkService;

        _actions[(ushort)AuthPackets.TS_AG_LOGIN_RESULT] = OnLoginResult;
        _actions[(ushort)AuthPackets.TS_AG_CLIENT_LOGIN] = OnAuthClientLoginResult;
    }
    
    public void Execute(Client client, IPacket packet)
    {
        if (!_actions.TryGetValue(packet.Id, out var action))
        {
            return;
        }

        action?.Invoke(client as AuthClient, packet);
    }
    
    private void OnLoginResult(AuthClient client, IPacket packet)
    {
        var msg = packet.GetDataStruct<TS_AG_LOGIN_RESULT>();

        if (msg.Result > 0)
        {
            throw new Exception("Failed to register to the Auth Server!");
        }

        client.Ready = true;
        
        _logger.Debug("Successfully registered to the Auth Server!");
    }
    
    private void OnAuthClientLoginResult(AuthClient authClient, IPacket packet)
    {
        var loginMsg = packet.GetDataStruct<TS_AG_CLIENT_LOGIN>();

        if (!_networkService.UnauthorizedGameClients.ContainsKey(loginMsg.Account))
        {
            _logger.Error("Account register failed for: {accountName}", loginMsg.Account);

            loginMsg.Result = (ushort)ResultCode.AccessDenied;
        }
        else
        {
            // Get the game client reference from the unauthorized client dictionary
            var gameClient = _networkService.UnauthorizedGameClients[loginMsg.Account];

            // Try to add the game client reference to authorized client dictionary
            if (!_networkService.AuthorizedGameClients.TryAdd(loginMsg.Account, gameClient))
            {
                gameClient.ConnectionInfo.AuthVerified = false;

                return;
            }

            // Remove the client reference from unauthorized client dictionary
            _networkService.UnauthorizedGameClients.Remove(loginMsg.Account);

            if (loginMsg.Result == (ushort)ResultCode.Success)
            {
                gameClient.ConnectionInfo.AccountName = loginMsg.Account;
                gameClient.ConnectionInfo.AccountId = loginMsg.AccountID;
                gameClient.ConnectionInfo.AuthVerified = true;
                gameClient.ConnectionInfo.PcBangMode = loginMsg.PcBangMode;
                gameClient.ConnectionInfo.EventCode = loginMsg.EventCode;
                gameClient.ConnectionInfo.Age = loginMsg.Age;
                gameClient.ConnectionInfo.ContinuousPlayTime = loginMsg.ContinuousPlayTime;
                gameClient.ConnectionInfo.ContinuousLogoutTime = loginMsg.ContinuousLogoutTime;
            }

            gameClient.ConnectionInfo.AuthVerified = true;

            gameClient.SendResult((ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH, loginMsg.Result);

            _logger.Debug("{clientTag} successfully verified and registered!", gameClient.ClientTag);
        }
    }
}