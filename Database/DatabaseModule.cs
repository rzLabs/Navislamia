using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Navislamia.Notification;
using Navislamia.Database.Contexts;
using Navislamia.Database.Enums;
using Dapper;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;

namespace Navislamia.Database
{
    public class DatabaseModule : IDatabaseService
    {
        INotificationService notificationSVC;

        WorldDbContext worldDbContext;
        PlayerDbContext playerDbContext;

        List<Task<int>> loadTasks = new();
        internal readonly WorldOptions _worldOptions; 
        internal readonly PlayerOptions _playerOptions; 

        public DatabaseModule(IOptions<WorldOptions> worldOptions, IOptions<PlayerOptions> playerOptions, INotificationService notificationService)
        {
            _worldOptions = worldOptions.Value;
            _playerOptions = playerOptions.Value;
            notificationSVC = notificationService;

            // TODO refactor context to be loaded from framework instead of manual creation (with migrations and entities)
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
