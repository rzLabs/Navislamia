using Navislamia.Database;
using Navislamia.Database.Entities;
using Navislamia.Database.Interfaces;
using Navislamia.Database.Repositories;
using Navislamia.Notification;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Navislamia.Database.Loaders
{
    public class PetLoader : RepositoryLoader, IRepositoryLoader
    {
        IDatabaseService dbSVC;

        public PetLoader(INotificationService notificationService, IDatabaseService databaseService) : base(notificationService)
        {
            dbSVC = databaseService;
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
