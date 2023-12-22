using Navislamia.Network.Enums;
using Navislamia.Network.Packets.Upload;
using System;
using System.Collections.Generic;
using Navislamia.Game.Network.Entities;
using Navislamia.Game.Network;
using Navislamia.Game.Network.Enums;

using Serilog;

namespace Navislamia.Network.Packets.Actions
{
    public class UploadActionService : IUploadActionService
    {
        IClientService _clientService;
        ILogger _logger = Log.ForContext<UploadActionService>();

        Dictionary<ushort, Func<UploadClientService, IPacket, int>> actions = new();

        public UploadActionService(IClientService clientService)
        {
            _clientService = clientService;

            actions[(ushort)UploadPackets.TS_US_LOGIN_RESULT] = OnLoginResult;
        }

        public void Execute(UploadClientService client, IPacket msg)
        {
            if (!actions.ContainsKey(msg.ID))
                return;

            actions[msg.ID]?.Invoke(client, msg);
        }

        public int OnLoginResult(UploadClientService client, IPacket msg)
        {
            var _msg = msg.GetDataStruct<TS_US_LOGIN_RESULT>();

            if (_msg.Result > 0)
            {
                _logger.Error("Failed to register to the Upload Server!");

                return 1;
            }

            _clientService.UploadReady = true;

            _logger.Debug("Successfully registered to the Upload Server!");

            return 0;
        }
    }
}
