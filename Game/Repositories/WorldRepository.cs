using Navislamia.Game.Contexts;
using Navislamia.Game.Models.Navislamia;

namespace Navislamia.Game.Repositories;

public class WorldRepository : IWorldRepository
{
    private readonly ArcadiaContext _arcadiaContext;

    public WorldRepository(ArcadiaContext arcadiaContext)
    {
        _arcadiaContext = arcadiaContext;
        LoadWorldIntoMemory();
    }
    
    public WorldEntity LoadWorldIntoMemory()
    {
        return new WorldEntity
        {
            ItemResources = _arcadiaContext.ItemResources,
            LevelResources = _arcadiaContext.LevelResources,
            ItemEffectResources = _arcadiaContext.ItemEffectResources,
            SetItemEffectResources = _arcadiaContext.SetItemEffectResources
        };
    }
}