using Navislamia.Game.Models.Arcadia.Enums;

namespace Navislamia.Game.Models.Arcadia;

public class SetItemEffectResourceEntity : Entity
{
    public SetParts Parts { get; set; } 
    public short[] BaseTypes { get; set; } // = new short[4]; // Base item Type of type ItemEffectInstant or ItemEffectPassive
    public decimal[,] BaseValues { get; set; } // = new decimal[4, 2];
    public short[] OptTypes { get; set; } // = new short[4]; // Extends Base item type by ItemEffectInstant or ItemEffectPassive
    public decimal[,] OptValues { get; set; } // = new decimal[4,2];
    
    public long? TextId { get; set; }
    public virtual StringResourceEntity Text { get; set; }
    
    public long? TooltipId { get; set; } 
    public virtual StringResourceEntity Tooltip { get; set; }
    
    public long? EffectId { get; set; } // not EffectResourceEntity

    // public virtual ICollection<ItemResourceEntity> Items { get; set; }
}
