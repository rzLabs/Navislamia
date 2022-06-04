using Navislamia.Database.Interfaces;
using Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Database; // TODO: this needs to be refactored

namespace Navislamia.Game.DbLoaders
{
    public class RepositoryLoader
    {
        public IDatabaseService DatabaseService;
        public INotificationService NotificationService;

        public RepositoryLoader(IDatabaseService databaseService, INotificationService notificationService)
        {
            DatabaseService = databaseService;
            NotificationService = notificationService;
        }
    }
}
