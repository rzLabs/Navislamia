using Database;
using Navislamia.Database.Entities;
using Navislamia.Database.Repositories;
using Notification;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Navislamia.Game.DbLoaders
{
    public class NpcLoader : RepositoryLoader
    {
        public List<NPCBase> Npc = new List<NPCBase>();

        public NpcLoader(IDatabaseService databaseService, INotificationService notificationService) : base(databaseService, notificationService) 
        {
            try {
                Npc = new NPCRepository(DatabaseService.WorldConnection).Fetch();
            }
            catch (Exception ex)
            {
                notificationService.WriteError("An exception has occured when executing the StringLoader!");
                notificationService.WriteException(ex);

                return;
            }
        }
    }
}