using DevConsole.Models.Arcadia.Enums;

namespace DevConsole.Models.Arcadia;

public class ItemEffectResourceEntity : Entity
{
    // TODO Check after stage-1 if this can be removed. Effects with multiple ordinal_ids could be linked to the item via EffectId[] e.g. ItemId 10 -> EffectIds [301003, 301004]
    public int OrdinalId { get; set; } // appears to be a sub-id to link two ItemEffects together e.g. Item with EffectId 301003 has two effects with different values
    public short EffectId { get; set; }
    public EffectType EffectType { get; set; }
    public short EffectLevel { get; set; } // is only 0 or 1 -> bool?
    public int[] Value { get; set; } = new int[20]; // DECIMAL(12,2);
    public int TooltipId { get; set; } 
    
    // public virtual StringResourceEntity Tooltip { get; set; }
    // public virtual EffectResourceEntity EffectResource { get; set; }

}