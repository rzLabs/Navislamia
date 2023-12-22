using Navislamia.Game.Models.Arcadia.Enums;

namespace Navislamia.Game.Models.Arcadia
{
    public class SetItemEffectResourceEntity
    {
        // SetId and SetParts composite key
        public int SetId { get; set; }
        public SetParts SetParts { get; set; } 
    
        public short[] BaseTypes { get; set; } // = new short[4]; // Base item Type of type ItemEffectInstant or ItemEffectPassive
        public decimal[,] BaseValues { get; set; } // = new decimal[4, 2];
        public short[] OptTypes { get; set; } // = new short[4]; // Extends Base item type by ItemEffectInstant or ItemEffectPassive
        public decimal[,] OptValues { get; set; } // = new decimal[4,2];
 
    
        public int TextId { get; set; }
        public int TooltipId { get; set; } 
        public int EffectId { get; set; }
    
        // public virtual StringResourceEntity Tooltip { get; set; }
        // public virtual StringResourceEntity Text { get; set; }
        // public virtual EffectResourceEntity Effect { get; set; }
    }
}