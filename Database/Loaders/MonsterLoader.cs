using Navislamia.Database;
using Navislamia.Database.Entities;
using Navislamia.Database.Interfaces;
using Navislamia.Database.Repositories;
using Navislamia.Notification;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Navislamia.Database.Loaders
{

    public class MonsterLoader : RepositoryLoader, IRepositoryLoader
    {
        IDatabaseService dbSVC;

        public MonsterLoader(INotificationService notificationService, IDatabaseService databaseService) : base(notificationService) 
        {
            dbSVC = databaseService;
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
