using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Navislamia.Game.Contexts;
using Navislamia.Game.Models.Arcadia;
using Navislamia.Game.Models.Navislamia;

namespace Navislamia.Game.Repositories;

public class WorldRepository : IWorldRepository
{
    private readonly DbContextOptions<ArcadiaContext> _options;


    public WorldRepository(DbContextOptions<ArcadiaContext> options)
    {
        _options = options;
    }
    
    public WorldEntity LoadWorldIntoMemory()
    {
        using var arcadiaContext = new ArcadiaContext(_options);
        return new WorldEntity
        {
            ItemResources = new List<ItemResourceEntity>(arcadiaContext.ItemResources),
            LevelResources =  new List<LevelResourceEntity>(arcadiaContext.LevelResources),
            ItemEffectResources = new List<ItemEffectResourceEntity>(arcadiaContext.ItemEffectResources),
            SetItemEffectResources =  new List<SetItemEffectResourceEntity>(arcadiaContext.SetItemEffectResources)
        };
    }
}