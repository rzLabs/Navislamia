using Database;
using Navislamia.Database.Entities;
using Navislamia.Database.Repositories;
using Notification;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.Linq;

namespace Navislamia.Game.DbLoaders
{
    class StringLoader : RepositoryLoader
    {
        public List<StringResource> Strings = new List<StringResource>();

        public StringLoader(IDatabaseService databaseService, INotificationService notificationService) : base(databaseService, notificationService) 
        {
            try {
                Strings = new StringRepository(DatabaseService.WorldConnection).Fetch();
            }
            catch (Exception ex)
            {
                notificationService.WriteError("An exception has occured when executing the StringLoader!");
                notificationService.WriteException(ex);

                return;
            }
        }
    }
}
