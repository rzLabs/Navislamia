using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Navislamia.Game.DataAccess.Contexts;
using Navislamia.Game.DataAccess.Repositories.Interfaces;
using Navislamia.Game.Models.Arcadia;
using Navislamia.Game.Models.Navislamia;

namespace Navislamia.Game.DataAccess.Repositories;

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