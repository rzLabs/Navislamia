using Navislamia.Database;
using Navislamia.World.Repositories.Entities;
using Navislamia.Notification;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Navislamia.World.Repositories.Loaders
{

    public class MonsterLoader : RepositoryLoader, IRepositoryLoader
    {
        IDatabaseModule dbSVC;

        public MonsterLoader(INotificationModule notificationModule, IDatabaseModule databaseModule) : base(notificationModule) 
        {
            dbSVC = databaseModule;
        }

        public List<IRepository> Init()
        {
            Tasks.Add(new MonsterSkillRepository(dbSVC.WorldConnection).Load());
            Tasks.Add(new MonsterItemDropRepository(dbSVC.WorldConnection).Load());
            Tasks.Add(new MonsterRepository(dbSVC.WorldConnection).Load());

            if (!Execute())
                return null;

            var monsterRepo = Repositories.Find(r=>r.Name == "MonsterResource").GetData<MonsterBase>();

            if (monsterRepo != null)
            {
                var skills = new List<MonsterSkill>(Repositories.Find(r=>r.Name == "MonsterSkillResource").GetData<MonsterSkill>());
                var drops = new List<MonsterItemDrop>(Repositories.Find(r=>r.Name == "MonsterDropTableResource").GetData<MonsterItemDrop>());

                foreach (var monster in monsterRepo)
                {
                    if (monster.SkillLinkID > 0)
                        monster.SkillInfoList = skills?.Find(s=>s.ID == monster.SkillLinkID);

                    if (monster.DropLinkID > 0)
                        monster.ItemDropList = drops?.Find(d=>d.ID == monster.DropLinkID);
                }
            }

            return this.Repositories;
        }
    }
}
