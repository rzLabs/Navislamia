// using System.Collections.Generic;
// using System.Data;
//
// using Navislamia.Notification;
// using Navislamia.Data.Repositories;
//
// namespace Navislamia.Data.Loaders;
//
// public class PetLoader : RepositoryLoader, IRepositoryLoader
// {
//
//     public PetLoader(INotificationModule notificationModule) : base(notificationModule)
//     {
//     }
//
//     public List<IEfRepository> Init()
//     {
//         // Tasks.Add(new PetRepository(_dbConnectionManager.WorldConnection).Load());
//
//         if (!Execute())
//             return null;
//
//         return Repositories;
//     }
// }
