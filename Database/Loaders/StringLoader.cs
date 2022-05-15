using Navislamia.Database.Repositories;
using Notification;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Database.Loaders
{
    class StringLoader : RepositoryLoader, IRepositoryLoader
    {
        IDbConnection dbConnection;

        public StringLoader(INotificationService notificationService, IDbConnection connection) : base(notificationService) 
        {
            dbConnection = connection;
        }

        public int Init()
        {
            Tasks.Add(Task.Run(() => new StringRepository(dbConnection).Load()));

            if (!Execute())
                return 1;

            return 0;
        }
    }
}
