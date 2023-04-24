using Navislamia.Database;
using Navislamia.Database.Interfaces;
using Navislamia.Database.Repositories;
using Navislamia.Notification;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Navislamia.Database.Loaders
{
    public class StringLoader : RepositoryLoader, IRepositoryLoader
    {
        IDatabaseService dbSVC;

        public StringLoader(INotificationService notificationService, IDatabaseService databaseService) : base(notificationService) 
        {
            dbSVC = databaseService;
        }

        public List<IRepository> Init()
        {
            Tasks.Add(Task.Run(() => new StringRepository(dbSVC.WorldConnection).Load()));

            if (!Execute())
                return null;

            return Repositories;
        }
    }
}
