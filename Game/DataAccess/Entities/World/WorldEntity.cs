using System.Collections.Generic;
using Navislamia.Game.DataAccess.Entities.Arcadia;

namespace Navislamia.Game.DataAccess.Entities.Navislamia;

public class WorldEntity
{
    public IEnumerable<ItemResourceEntity> ItemResources { get; set; }
    public IEnumerable<ItemEffectResourceEntity> ItemEffectResources { get; set; }
    public IEnumerable<SetItemEffectResourceEntity> SetItemEffectResources { get; set; }
    public IEnumerable<LevelResourceEntity> LevelResources { get; set; }
}
