using System;
using System.Collections.Generic;
using System.Text;

using System.Data.SqlClient;
using System.Data;

using System.Threading.Tasks;

using System.IO;
using System.Linq;
using System.Reflection;

using Configuration;
using Notification;
using Serilog.Events;
using Navislamia.Data;
using Navislamia.Database.Contexts;
using Navislamia.Database.Repositories;
using Navislamia.Database.Entities;

using Dapper;
using Navislamia.Database.Interfaces;
using Navislamia.Database.Enums;

namespace Database
{
    public class DatabaseModule : IDatabaseService
    {
        IConfigurationService configSVC;
        INotificationService notificationSVC;
        IDataService dataSVC;

        WorldDbContext worldDbContext;
        PlayerDbContext playerDbContext;

        HashSet<IRepository> worldRepositories = new HashSet<IRepository>();

        public DatabaseModule() { }

        public DatabaseModule(IConfigurationService configurationService, INotificationService notificationService, IDataService dataService)
        {
            configSVC = configurationService;
            notificationSVC = notificationService;
            dataSVC = dataService;

            worldDbContext = new WorldDbContext(configSVC);
            playerDbContext = new PlayerDbContext(configSVC);
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

        public async Task<int> LoadRepositories() // TODO: Arcadia table loads go here
        {
            List<string> successMsgs = new List<string>();

            successMsgs.Add("Successfully loaded database repositories!");

            try
            {
                using IDbConnection dbConnection = worldDbContext.CreateConnection();

                var repo = await new StringRepository(dbConnection).Init();

                successMsgs.Add($"- [yellow]{repo.Count}[/] rows loaded from {repo.Name}");
            }
            catch (Exception ex)
            {
                notificationSVC.WriteError("An error occured while attempting to load world repositories!");
                notificationSVC.WriteException(ex);

                return 1;
            }

            notificationSVC.WriteSuccess(successMsgs.ToArray(), true);

            return 0;
        }
    }
}
