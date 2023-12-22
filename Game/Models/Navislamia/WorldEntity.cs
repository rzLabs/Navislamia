using System.Collections.Generic;
using Navislamia.Game.Models.Arcadia;

namespace Navislamia.Game.Models.Navislamia;

public class WorldEntity
{
    public IEnumerable<ItemResourceEntity> ItemResources { get; set; }
    public IEnumerable<ItemEffectResourceEntity> ItemEffectResources { get; set; }
    public IEnumerable<SetItemEffectResourceEntity> SetItemEffectResources { get; set; }
    public IEnumerable<LevelResourceEntity> LevelResources { get; set; }
}