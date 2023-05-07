using Navislamia.Database.Entities;
using Navislamia.Database.Interfaces;
using Navislamia.Database.Repositories;
using Navislamia.Notification;
using System.Collections.Generic;

namespace Navislamia.Database.Loaders
{
    public class ItemLoader : RepositoryLoader, IRepositoryLoader
    {
        IDatabaseModule dbSVC;

        public ItemLoader(INotificationModule notificationModule, IDatabaseModule databaseModule) : base(notificationModule)
        {
            dbSVC = databaseModule;
        }

        public List<IRepository> Init()
        {
            Tasks.Add(new ItemEffectRepository(dbSVC.WorldConnection).Load());
            Tasks.Add(new SetItemEffectRepository(dbSVC.WorldConnection).Load());

            if (!Execute())
                return null;

            var effects = new List<EffectInfo>(Repositories[0].GetData<EffectInfo>());
            var itemEffects = Repositories[1].GetData<SetItemEffect>();

            foreach (SetItemEffect itemEffect in itemEffects)
            {
                for (int i = 0; i < SetItemEffect.MAX_OPTIONS; i++)
                {
                    if (itemEffect.EffectID > 0)
                        itemEffect.Effects = effects.FindAll(e => e.EffectID == itemEffect.EffectID);
                }
            }

            return this.Repositories;
        }
    }
}
