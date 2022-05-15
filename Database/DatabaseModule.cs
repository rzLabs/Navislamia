using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Configuration;
using Notification;

using Navislamia.Database.Contexts;
using Navislamia.Database.Enums;
using Navislamia.Database.Interfaces;
using Navislamia.Database.Repositories;

using Dapper;
using System.Text;
using Navislamia.Database.Loaders;

namespace Database
{
    public class DatabaseModule : IDatabaseService
    {
        IConfigurationService configSVC;
        INotificationService notificationSVC;

        WorldDbContext worldDbContext;
        PlayerDbContext playerDbContext;

        List<Task<int>> loadTasks = new List<Task<int>>();

        public DatabaseModule() { }

        public DatabaseModule(IConfigurationService configurationService, INotificationService notificationService)
        {
            configSVC = configurationService;
            notificationSVC = notificationService;

            worldDbContext = new WorldDbContext(configSVC);
            playerDbContext = new PlayerDbContext(configSVC);

            setLoadTasks();
        }

        void setLoadTasks()
        {
            loadTasks.Add(Task.Run(() => new StringLoader(notificationSVC, worldDbContext.CreateConnection()).Init()));
        }

        public bool Init()
        {          
            Task loadTask = Task.WhenAll(loadTasks);

            try
            {
                loadTask.Wait();
            }
            catch (Exception ex) { }

            if (!loadTask.IsCompletedSuccessfully)
            {
                foreach (Task<int> task in loadTasks)
                {
                    if (task.IsFaulted)
                    {
                        notificationSVC.WriteError($"{task.GetType().Name} task has failed!");
                        notificationSVC.WriteException(task.Exception);
                    }
                }

                return false;
            }

            return true;
        }

        public async Task<int> ExecuteScalar(string command, DbContextType type = DbContextType.Player)
        {
            using IDbConnection dbConnection = (type == DbContextType.Player) ? playerDbContext.CreateConnection() : worldDbContext.CreateConnection();

            return await dbConnection.ExecuteScalarAsync<int>(command);
        }

        public async Task<int> ExecuteStoredProcedure<T>(string storedProcedure, T parameters)
        {
            using IDbConnection dbConnection = playerDbContext.CreateConnection();

            return await dbConnection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
