using Navislamia.Notification;

namespace Navislamia.World
{
    public class WorldModule : IWorldModule
    {
        INotificationModule notificationSVC;

        public WorldModule(INotificationModule notificationModule)
        {
            notificationSVC = notificationModule;
        }

    }
}
