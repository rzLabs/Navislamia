using System;
using System.Collections.Generic;
using System.Linq;
using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Packets.Auth;
using Serilog;

namespace Navislamia.Game.Network.Entities.Actions;

public class AuthActions
{
    private readonly ILogger _logger = Log.ForContext<AuthActions>();
    private readonly Dictionary<ushort, Action<AuthClient, IPacket>> _actions = new();
    public List<GameClient> GameClients { get; set; }
    private GameClient RelatedGameClient { get; set; }

    public AuthActions(List<GameClient> gameClients)
    {
        GameClients = gameClients;
        _actions[(ushort)AuthPackets.TS_AG_LOGIN_RESULT] = OnLoginResult;
        _actions[(ushort)AuthPackets.TS_AG_CLIENT_LOGIN] = OnAuthClientLoginResult;
    }
    
    public void SetAffectedGameClient(GameClient client)
    {
        RelatedGameClient = client;
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
        
        _logger.Debug("Successfully registered to the Auth Server!");
    }
    
    private void OnAuthClientLoginResult(AuthClient authClient, IPacket packet)
    {
        var userLogin = packet.GetDataStruct<TS_AG_CLIENT_LOGIN>();
        if (RelatedGameClient.Authorized)
        {
            _logger.Error("Account register failed for: {accountName}", userLogin.Account);
            var gameClient = GameClients.FirstOrDefault(c => c.AccountName == userLogin.Account);
            if (gameClient != null)
            {
                GameClients.Remove(gameClient);
            }
            // TODO: SendLogoutToAuth user is already islogged in, wrong credentials etc -> send logout to auth
            userLogin.Result = (ushort)ResultCode.AccessDenied;
        }
        else
        {
            if (userLogin.Result == (ushort)ResultCode.Success)
            {
                RelatedGameClient.AccountName = userLogin.Account;
                RelatedGameClient.AccountId = userLogin.AccountID;
                RelatedGameClient.AuthVerified = true;
                RelatedGameClient.PcBangMode = userLogin.PcBangMode;
                RelatedGameClient.EventCode = userLogin.EventCode;
                RelatedGameClient.Age = userLogin.Age;
                RelatedGameClient.ContinuousPlayTime = userLogin.ContinuousPlayTime;
                RelatedGameClient.ContinuousLogoutTime = userLogin.ContinuousLogoutTime;
            }
            RelatedGameClient.Authorized = true;
            GameClients.Add(RelatedGameClient);
        }

        RelatedGameClient.SendResult((ushort)GamePackets.TM_CS_ACCOUNT_WITH_AUTH, userLogin.Result);
    }
}