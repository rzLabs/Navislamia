using Configuration;
using Navislamia.Network.Enums;
using Network;
using Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Navislamia.Network.Packets.Auth;

namespace Navislamia.Network.Packets
{
    public interface IAuthActionService
    {
        public int Execute(ISerializablePacket msg);
    }

    public class AuthActions : IAuthActionService
    {
        IConfigurationService configSVC;
        INotificationService notificationSVC;

        Dictionary<ushort, Func<ISerializablePacket, int>> actions = new Dictionary<ushort, Func<ISerializablePacket, int>>();

        public AuthActions(IConfigurationService configService, INotificationService notificationService)
        {
            configSVC = configService;
            notificationSVC = notificationService;

            actions[(ushort)AuthPackets.TS_AG_LOGIN_RESULT] = OnLoginResult; // TODO: example, remove me!
        }

        public int Execute(ISerializablePacket msg)
        {
            if (!actions.ContainsKey(msg.ID))
                return 1;

            return actions[msg.ID]?.Invoke(msg) ?? 2;
        }

        public int OnLoginResult(ISerializablePacket msg)
        {
            var _msg = msg as TS_AG_LOGIN_RESULT;

            if (_msg.Result > 0)
            {
                notificationSVC.WriteError("Failed to register to the Auth Server!");

                return 1;
            }

            notificationSVC.WriteSuccess("Successfully registered to the Auth Server!");

            return 0;
        }
    }
}
