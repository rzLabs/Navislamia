using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;

using Navislamia.Database;
using Navislamia.Notification;
using Navislamia.Data.Interfaces;
using Navislamia.Data.Repositories;

namespace Navislamia.Data.Loaders;

public class StringLoader : RepositoryLoader, IRepositoryLoader
{
    DbConnectionManager _dbConnectionManager;

    public StringLoader(INotificationModule notificationModule, DbConnectionManager dbConnectionManager) : base(notificationModule) 
    {
        _dbConnectionManager = dbConnectionManager;
    }

    public List<IRepository> Init()
    {
        Tasks.Add(Task.Run(() => new StringRepository(_dbConnectionManager.WorldConnection).Load()));

        if (!Execute())
            return null;

        return Repositories;
    }
}
