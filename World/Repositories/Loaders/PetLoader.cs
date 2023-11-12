using Navislamia.Database;
using Navislamia.Notification;
using System.Collections.Generic;

namespace Navislamia.World.Repositories.Loaders
{
    public class PetLoader : RepositoryLoader, IRepositoryLoader
    {
        IDatabaseModule dbSVC;

        public PetLoader(INotificationModule notificationModule, IDatabaseModule databaseModule) : base(notificationModule)
        {
            dbSVC = databaseModule;
        }

        public List<IRepository> Init()
        {
            Tasks.Add(new PetRepository(dbSVC.WorldConnection).Load());

            if (!Execute())
                return null;

            return Repositories;
        }
    }
}
