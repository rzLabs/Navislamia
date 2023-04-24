using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Navislamia.Database.Contexts;
using Navislamia.Database.Enums;
using Dapper;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Notification;

namespace Navislamia.Database
{
    public class DatabaseModule : IDatabaseService
    {
        INotificationService notificationSVC;
        private readonly WorldOptions _worldOptions;
        private readonly PlayerOptions _playerOptions;
        WorldDbContext worldDbContext;
        PlayerDbContext playerDbContext;

        List<Task<int>> loadTasks = new();

        public DatabaseModule() { }

        public DatabaseModule(IOptions<WorldOptions> worldOptions, IOptions<PlayerOptions> playerOptions, INotificationService notificationService)
        {
            notificationSVC = notificationService;

            worldDbContext = new WorldDbContext(worldOptions);
            playerDbContext = new PlayerDbContext(playerOptions);
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
