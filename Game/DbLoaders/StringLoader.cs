using Database;
using Navislamia.Database.Repositories;
using Notification;
using System.Threading.Tasks;

namespace Navislamia.Game.DbLoaders
{
    class StringLoader : RepositoryLoader
    {
        IDatabaseService dbSVC;

        public StringLoader(INotificationService notificationService, IDatabaseService databaseService) : base(notificationService) 
        {
            dbSVC = databaseService;
        }

        public async Task<RepositoryLoader> Init()
        {
            Tasks.Add(Task.Run(() => new StringRepository(dbSVC.WorldConnection).Load()));

            if (!await Execute())
                return null;

            return this;
        }
    }
}
