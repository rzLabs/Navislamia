using Database;
using Navislamia.Database.Entities;
using Navislamia.Database.Repositories;
using Notification;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Navislamia.Game.DbLoaders
{
    public class MonsterLoader : RepositoryLoader
    {
        public List<MonsterSkill> Skills;
        public List<MonsterItemDrop> Drops;

        public List<MonsterBase> Monsters;

        public MonsterLoader(IDatabaseService databaseService, INotificationService notificationService) : base(databaseService, notificationService) 
        {
            Skills = new MonsterSkillRepository(DatabaseService.WorldConnection).Fetch();
            Drops = new MonsterItemDropRepository(DatabaseService.WorldConnection).Fetch();
            Monsters = new MonsterRepository(DatabaseService.WorldConnection).Fetch();
            
            if (Monsters is not null)
            {
                foreach (var monster in Monsters)
                {
                    if (monster.SkillLinkID > 0)
                        monster.SkillInfoList = Skills?.Find(s=>s.ID == monster.SkillLinkID);

                    if (monster.DropLinkID > 0)
                        monster.ItemDropList = Drops?.Find(d=>d.ID == monster.DropLinkID);            
                }
            }
        }
    }
}
