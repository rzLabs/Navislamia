using DevConsole.Models.Arcadia.Enums;

namespace DevConsole.Models.Arcadia;

public class SetItemEffectResourceEntity
{
    public int SetId { get; set; } // not unique 
    public SetPartFlag SetParts { get; set; } 
    
    public ItemEffectInstantType[] BaseType { get; set; } = new ItemEffectInstantType[2]; // Base item Type -> unused? only entries with optvar have values. Might leave for future functionality
    public decimal[,] BaseVar { get; set; } = new decimal[2, 4]; // DECIMAL(12,2) 
    public ItemEffectInstantType[] OptType { get; set; } = new ItemEffectInstantType[2]; // Extends Base item type by ethereal stuff
    public decimal[,] OptVar { get; set; } = new decimal[2,4]; // DECIMAL(12,2)
    
    public int TextId { get; set; }
    public int TooltipId { get; set; } 
    public int EffectId { get; set; }
    
    // public virtual StringResourceEntity Tooltip { get; set; }
    // public virtual StringResourceEntity Text { get; set; }
    // public virtual EffectResourceEntity Effect { get; set; }
    


}