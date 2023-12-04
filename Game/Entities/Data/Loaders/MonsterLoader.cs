using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;


using Navislamia.Database;
using Navislamia.Notification;
using Navislamia.Data.Interfaces;
using Navislamia.Data.Entities;
using Navislamia.Data.Repositories;

namespace Navislamia.Data.Loaders;

public class MonsterLoader : RepositoryLoader, IRepositoryLoader
{

    public MonsterLoader(INotificationModule notificationModule) : base(notificationModule) 
    {
    }

    public List<IRepository> Init()
    {
    //     Tasks.Add(new MonsterSkillRepository(_dbConnectionManager.WorldConnection).Load());
    //     Tasks.Add(new MonsterItemDropRepository(_dbConnectionManager.WorldConnection).Load());
    //     Tasks.Add(new MonsterRepository(_dbConnectionManager.WorldConnection).Load());

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
