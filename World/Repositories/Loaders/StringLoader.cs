using Navislamia.Database;
using Navislamia.Notification;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Navislamia.World.Repositories.Loaders
{
    public class StringLoader : RepositoryLoader, IRepositoryLoader
    {
        IDatabaseModule dbSVC;

        public StringLoader(INotificationModule notificationModule, IDatabaseModule databaseModule) : base(notificationModule) 
        {
            dbSVC = databaseModule;
        }

        public List<IRepository> Init()
        {
            Tasks.Add(Task.Run(() => new StringRepository(dbSVC).Load()));

            if (!Execute())
                return null;

            return Repositories;
        }
    }
}
