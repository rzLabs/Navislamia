using Configuration;
using Navislamia.Network.Entities;
using Navislamia.Network.Enums;
using Navislamia.Network.Packets.Actions.Interfaces;
using Navislamia.Network.Packets.Upload;
using Navislamia.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets.Actions
{
    public class UploadActions : IUploadActionService
    {
        IConfigurationService configSVC;
        INotificationService notificationSVC;

        Dictionary<ushort, Func<Client, ISerializablePacket, int>> actions = new Dictionary<ushort, Func<Client, ISerializablePacket, int>>();

        public UploadActions(IConfigurationService configService, INotificationService notificationService)
        {
            configSVC = configService;
            notificationSVC = notificationService;

            actions[(ushort)UploadPackets.TS_US_LOGIN_RESULT] = OnLoginResult;
        }

        public int Execute(Client client, ISerializablePacket msg)
        {
            if (!actions.ContainsKey(msg.ID))
                return 1;

            return actions[msg.ID]?.Invoke(client, msg) ?? 2;
        }

        public int OnLoginResult(Client client, ISerializablePacket msg)
        {
            var _msg = msg as TS_US_LOGIN_RESULT;

            if (_msg.Result > 0)
            {
                notificationSVC.WriteError("Failed to register to the Upload Server!");

                return 1;
            }

            client.Ready = true;

            notificationSVC.WriteSuccess("Successfully registered to the Upload Server!");

            return 0;
        }
    }
}
