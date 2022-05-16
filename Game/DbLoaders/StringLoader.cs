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
    class StringLoader : RepositoryLoader
    {
        IDbConnection dbConnection;

        public StringLoader(INotificationService notificationService, IDbConnection connection) : base(notificationService) 
        {
            dbConnection = connection;
        }

        public async Task<RepositoryLoader> Init()
        {
            Tasks.Add(Task.Run(() => new StringRepository(dbConnection).Load()));

            if (!await Execute())
                return null;

            return this;
        }
    }
}
