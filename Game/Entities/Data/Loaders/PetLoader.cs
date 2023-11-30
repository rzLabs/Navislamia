using System.Collections.Generic;
using System.Data;

using Navislamia.Notification;
using Navislamia.Data.Interfaces;
using Navislamia.Data.Repositories;
using Navislamia.Database;

namespace Navislamia.Data.Loaders;

public class PetLoader : RepositoryLoader, IRepositoryLoader
{
    DbConnectionManager _dbConnectionManager;

    public PetLoader(INotificationModule notificationModule, DbConnectionManager dbConnectionManager) : base(notificationModule)
    {
        _dbConnectionManager = dbConnectionManager;
    }

    public List<IRepository> Init()
    {
        Tasks.Add(new PetRepository(_dbConnectionManager.WorldConnection).Load());

        if (!Execute())
            return null;

        return Repositories;
    }
}
