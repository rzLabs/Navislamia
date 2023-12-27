using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Game.Contexts;
using Navislamia.Game.Models.Arcadia;
using Navislamia.Game.Models.Navislamia;

namespace Navislamia.Game.Repositories;

public class WorldRepository : IWorldRepository
{
    private readonly DbContextOptions<ArcadiaContext> _options;
    private readonly IOptions<DatabaseOptions> _dbOptions;

    public WorldRepository(DbContextOptions<ArcadiaContext> options, IOptions<DatabaseOptions> dbOptions)
    {
        _options = options;
        _dbOptions = dbOptions;
    }
    
    public WorldEntity LoadWorldIntoMemory()
    {
        using var arcadiaContext = new ArcadiaContext(_options, _dbOptions);
        return new WorldEntity
        {
            ItemResources = new List<ItemResourceEntity>(arcadiaContext.ItemResources),
            LevelResources =  new List<LevelResourceEntity>(arcadiaContext.LevelResources),
            ItemEffectResources = new List<ItemEffectResourceEntity>(arcadiaContext.ItemEffectResources),
            SetItemEffectResources =  new List<SetItemEffectResourceEntity>(arcadiaContext.SetItemEffectResources)
        };
    }
}