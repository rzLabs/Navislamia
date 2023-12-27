using System;
using System.Collections.Generic;
using Navislamia.Game.Network.Packets;
using Serilog;

namespace Navislamia.Game.Network.Entities.Actions;

public class UploadActions
{
    private readonly ILogger _logger = Log.ForContext<UploadActions>();
    private readonly Dictionary<ushort, Action<UploadClient, IPacket>> _actions = new();

    public UploadActions()
    {
        _actions[(ushort)UploadPackets.TS_US_LOGIN_RESULT] = OnLoginResult;
    }
    
    private void OnLoginResult(UploadClient client, IPacket packet)
    {
        var msg = packet.GetDataStruct<TS_US_LOGIN_RESULT>();

        if (msg.Result > 0)
        {
            _logger.Error("Failed to register to the Upload Server!");
            throw new Exception();

        }

        _logger.Debug("Successfully registered to the Upload Server!");
    }
    
    public void Execute(UploadClient client, IPacket packet)
    {
        if (!_actions.TryGetValue(packet.ID, out var action))
        {
            return;
        }
            
        action?.Invoke(client, packet);
    }
}