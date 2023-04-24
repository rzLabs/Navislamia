using Configuration;
using Navislamia.Notification;
using System;

namespace Navislamia.World
{
    public class WorldModule : IWorldService
    {
        IConfigurationService configSVC;
        INotificationService notificationSVC;

        public WorldModule(IConfigurationService configurationService, INotificationService notificationService)
        {
            configSVC = configurationService;
            notificationSVC = notificationService;
        }

    }
}
