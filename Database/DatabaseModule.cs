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

namespace Database
{
    public class DatabaseModule : IDatabaseService
    {
        IConfigurationService configSVC;
        INotificationService notificationSVC;
        IDataService dataSVC;

        WorldDbContext worldDbContext;
        PlayerDbContext playerDbContext;

        public DatabaseModule() { }

        public DatabaseModule(IConfigurationService configurationService, INotificationService notificationService, IDataService dataService)
        {
            configSVC = configurationService;
            notificationSVC = notificationService;
            dataSVC = dataService;

            worldDbContext = new WorldDbContext(configSVC);
            playerDbContext = new PlayerDbContext(configSVC);
        }

        public int Init() // TODO: arcadia table loading logic should occur here
        {
            List<StringResource> stringRepo = null;

            try
            {

                stringRepo = new StringResourceRespository(notificationSVC, worldDbContext).Get();
            }
            catch (Exception ex)
            {
                notificationSVC.WriteException(ex);

                return 1;
            }

            dataSVC.Set<List<StringResource>>("strings", stringRepo);

            int stringCnt = dataSVC.Get<List<StringResource>>("strings").Count; // TODO: these should say their counts individually not within the success statement

            notificationSVC.WriteSuccess(new string[] { "Successfully started the database server", $"- [green]{stringCnt}[/] strings loaded!" }, true);

            return 0;
        }

        public enum TargetType
        {
            Player,
            World
        }

        public async Task<int> ExecuteScalar(string command, TargetType type = TargetType.Player)
        {
            using IDbConnection dbConnection = (type == TargetType.Player) ? playerDbContext.CreateConnection() : worldDbContext.CreateConnection();

            return await dbConnection.ExecuteScalarAsync<int>(command);
        }

        public async Task<int> ExecuteStoredProcedure<T>(string storedProcedure, T parameters)
        {
            using IDbConnection dbConnection = playerDbContext.CreateConnection();

            return await dbConnection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
