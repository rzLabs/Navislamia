using Configuration;
using Navislamia.Notification;
using System;

namespace Navislamia.World
{
    public class WorldModule : IWorldService
    {
        INotificationService notificationSVC;

        public WorldModule(INotificationService notificationService)
        {
            notificationSVC = notificationService;
        }

    }
}
