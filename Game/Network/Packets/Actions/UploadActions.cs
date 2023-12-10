using Navislamia.Network.Entities;
using Navislamia.Network.Enums;
using Navislamia.Network.Packets.Upload;
using Navislamia.Notification;
using System;
using System.Collections.Generic;

namespace Navislamia.Network.Packets.Actions
{
    public class UploadActions
    {
        INotificationModule notificationSVC;

        Dictionary<ushort, Func<ClientService<UploadClientEntity>, IPacket, int>> actions = new();

        public UploadActions(INotificationModule notificationModule)
        {
            notificationSVC = notificationModule;
            actions[(ushort)UploadPackets.TS_US_LOGIN_RESULT] = OnLoginResult;
        }

        public int Execute(ClientService<UploadClientEntity> client, IPacket msg)
        {
            if (!actions.ContainsKey(msg.ID))
                return 1;

            return actions[msg.ID]?.Invoke(client, msg) ?? 2;
        }

        public int OnLoginResult(ClientService<UploadClientEntity> client, IPacket msg)
        {
            var _msg = msg.GetDataStruct<TS_US_LOGIN_RESULT>();

            if (_msg.Result > 0)
            {
                notificationSVC.WriteError("Failed to register to the Upload Server!");

                return 1;
            }

            client.GetEntity().Ready = true;

            notificationSVC.WriteSuccess("Successfully registered to the Upload Server!");

            return 0;
        }
    }
}
