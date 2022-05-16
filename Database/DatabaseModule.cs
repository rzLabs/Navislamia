using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Configuration;
using Notification;

using Navislamia.Database.Contexts;
using Navislamia.Database.Enums;

using Dapper;
using System.Text;

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
        }

        public IDbConnection WorldConnection => worldDbContext.CreateConnection();

        public IDbConnection PlayerConnection => playerDbContext.CreateConnection();

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
