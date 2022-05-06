using Configuration;
using Navislamia.Network.Enums;
using Network;
using Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets
{
    public interface IGameActionService
    {
        public int Execute(ISerializablePacket msg);
    }

    public class GameActions : IGameActionService
    {
        IConfigurationService configSVC;
        INotificationService notificationSVC;

        Dictionary<ushort, Func<ISerializablePacket, int>> actions = new Dictionary<ushort, Func<ISerializablePacket, int>>();

        public GameActions(IConfigurationService configService, INotificationService notificationService)
        {
            configSVC = configService;
            notificationSVC = notificationService;
        }

        public int Execute(ISerializablePacket msg)
        {
            if (!actions.ContainsKey(msg.ID))
                return 1;

            return actions[msg.ID]?.Invoke(msg) ?? 2;
        }

    }
}
