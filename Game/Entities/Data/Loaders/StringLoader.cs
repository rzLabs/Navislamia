using System.Collections.Generic;
using Navislamia.Game.Entities.Data.Interfaces;
using Navislamia.Notification;

namespace Navislamia.Data.Loaders;

public class StringLoader : RepositoryLoader, IRepositoryLoader
{

    public StringLoader(INotificationModule notificationModule) : base(notificationModule) 
    {
    }

    public List<IEfRepository> Init()
    {
        // Tasks.Add(Task.Run(() => new StringRepository(_dbConnectionManager.WorldConnection).Load()));

        if (!Execute())
            return null;

        return Repositories;
    }
}
