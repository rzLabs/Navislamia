using Navislamia.Database;
using Navislamia.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.World.Repositories.Loaders
{
    public class ETCLoader : RepositoryLoader, IRepositoryLoader
    {
        IDatabaseModule dbSVC;

        public ETCLoader(INotificationModule notificationModule, IDatabaseModule databaseModule) : base(notificationModule)
        {
            dbSVC = databaseModule;
        }

        public List<IRepository> Init()
        {

            // TODO: MarketData
            Tasks.Add(Task.Run(() => new LevelExpRepository(dbSVC).Load()));
            // TODO: SummonLevelUpTable
            // TODO: Job
            // TODO: State
            // TODO: BannedWord
            // TODO: Enhance
            // TODO: AutoAccount
            // TODO: GlobalVariable

            if (!Execute())
                return null;

            return Repositories;
        }
    }
}
