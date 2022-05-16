using Navislamia.Database.Interfaces;
using Navislamia.Database.Repositories;
using Notification;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.DbLoaders
{

    public class MonsterLoader : RepositoryLoader
    {
        IDbConnection dbConnection;

        public MonsterLoader(INotificationService notificationService, IDbConnection connection) : base(notificationService) 
        {
            dbConnection = connection;
        }

        public async Task<RepositoryLoader> Init()
        {
            Tasks.Add(Task.Run(() => new MonsterSkillRepository(dbConnection).Load()));
            //Tasks.Add(Task.Run(() => new MonsterRepository(dbConnection).Load()));

            if (!await Execute())
                return null;

            return this;
        }

        // TODO:
        //LoadMonsterData(); -> Monster, MonsterSkill, MonsterDrop
        //LoadSummonData();
        //LoadSummonLevelBonusData();
        //LoadDropGroupData();
        //LoadSummonNames();
    }
}
