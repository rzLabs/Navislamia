using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Navislamia.Configuration.Options;
using Navislamia.Database.Contexts;
using Navislamia.Notification;

namespace Navislamia.Database;

public class DbConnectionManager
{
    WorldDbContext worldDbContext;
    PlayerDbContext playerDbContext;

    internal readonly WorldOptions _worldOptions;
    internal readonly PlayerOptions _playerOptions;

    public DbConnectionManager(IOptions<WorldOptions> worldOptions, IOptions<PlayerOptions> playerOptions)
    {
        _worldOptions = worldOptions.Value;
        _playerOptions = playerOptions.Value;

        // TODO refactor context to be loaded from framework instead of manual creation (with migrations and entities)
        worldDbContext = new WorldDbContext(worldOptions);
        playerDbContext = new PlayerDbContext(playerOptions);
    }

    public IDbConnection WorldConnection => worldDbContext.CreateConnection();

    public IDbConnection PlayerConnection => playerDbContext.CreateConnection();

}
