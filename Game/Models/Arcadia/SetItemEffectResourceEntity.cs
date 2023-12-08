using Navislamia.Game.Models.Arcadia.Enums;

namespace Navislamia.Game.Models.Arcadia
{
    public class SetItemEffectResourceEntity
    {
        public int SetId { get; set; } // not unique 
        public SetPartFlag SetParts { get; set; } 
    
        public short[] BaseType { get; set; } = new short[2]; 
        public decimal[,] BaseVar { get; set; } = new decimal[2, 4];
        public short[] OptType { get; set; } = new short[2];
        public decimal[,] OptVar { get; set; } = new decimal[2,4]; 
    
        public int TextId { get; set; }
        public int TooltipId { get; set; } 
        public int EffectId { get; set; }
    
        // public virtual StringResourceEntity Tooltip { get; set; }
        // public virtual StringResourceEntity Text { get; set; }
        // public virtual EffectResourceEntity Effect { get; set; }
    }
}