using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Navislamia.Game.Network.Entities;
using Navislamia.Game.Network.Interfaces;

namespace Navislamia.Game.Network.Packets.Actions;

public class UploadActionService : IUploadActionService
{
    // private readonly ILogger<UploadActionService> _logger;

    private readonly Dictionary<ushort, Action<UploadClient, IPacket>> _actions = new();

    public UploadActionService()
    {
        // _logger = logger;

        _actions[(ushort)UploadPackets.TS_US_LOGIN_RESULT] = OnLoginResult;
    }

    public void Execute(UploadClient client, IPacket packet)
    {
        if (!_actions.TryGetValue(packet.ID, out var action))
        {
            return;
        }
            
        action?.Invoke(client, packet);
    }

    public void OnLoginResult(UploadClient client, IPacket packet)
    {
        var msg = packet.GetDataStruct<TS_US_LOGIN_RESULT>();

        if (msg.Result > 0)
        {
            // _logger.LogError("Failed to register to the Auth Server!");
            throw new Exception();

        }

        // _logger.LogDebug("Successfully registered to the Upload Server!");
        Console.WriteLine("Successfully registered to the Upload Server!");

    }
}