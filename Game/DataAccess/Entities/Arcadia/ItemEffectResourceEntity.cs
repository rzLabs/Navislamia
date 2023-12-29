
using Navislamia.Game.DataAccess.Entities.Enums;

namespace Navislamia.Game.DataAccess.Entities.Arcadia;

public class ItemEffectResourceEntity : Entity
{
    // TODO Check after stage-1 if this can be removed.
    // appears to be a sub-id linking multiple ItemEffects together
    // e.g. Item with EffectId 301003 has two effects with different values
    // Effects with multiple ordinal_ids could be linked to the item via EffectId[]
    // e.g. ItemId 10 -> EffectIds [301003, 301004] where 301003 would represent ordinalId 1 and 301004 ordinalId 2 etc.
    public long OrdinalId { get; set; }
    
    public long TooltipId { get; set; }
    public virtual StringResourceEntity Tooltip { get; set; }
    
    public EffectTrigger EffectTrigger { get; set; } //previously effect_id but its an enum, not an entity
    
    public EffectType EffectType { get; set; }
    public bool EffectLevel { get; set; }
    public decimal[] Values { get; set; } // 20
}
