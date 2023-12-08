using Navislamia.Game.Models.Arcadia.Enums;
using ItemGroup = Navislamia.Game.Models.Arcadia.Enums.ItemGroup;
using ItemType = Navislamia.Game.Models.Arcadia.Enums.ItemType;
using ItemWearType = Navislamia.Game.Models.Arcadia.Enums.ItemWearType;

namespace Navislamia.Game.Models.Arcadia
{
    public class ItemResourceEntity : Entity
    {
        public ItemBaseType ItemBaseType { get; set; } 
        public ItemType ItemType { get; set; } 
        public ItemGroup Group { get; set; } 
        public ItemWearType WearType { get; set; } 
        public SetPartType SetPartFlag { get; set; } 
        public ItemStatus Status { get; set; }
        public ItemClassRestriction ClassRestriction { get; set; }
        public ItemRestriction Restriction { get; set; }
        public ItemUseFlag ItemUseFlag { get; set; }
        public ItemDecreaseTimeType DecreaseType { get; set; }
    
        public int Grade { get; set; } // 0 - 7 -> how much dura is consumed when attacking/being attacked
        public int Rank { get; set; }
        public int Level { get; set; }
        public int Enhance { get; set; } // 0 - 20
        public int SocketCount { get; set; } // 0 - 4
        public short JobDepth { get; set; }
        public int UseMinLevel { get; set; }
        public int UseMaxLevel { get; set; }
        public int TargetMinLevel { get; set; }
        public int TargetMaxLevel { get; set; }
        public decimal Range { get; set; }
        public decimal Weight { get; set; }
        public int Price { get; set; }
        public int HuntaholicPoint { get; set; }
        public int EtherealDurability { get; set; }
        public int Endurance { get; set; }
        public int Material { get; set; } // unused?
        public int AvailablePeriod { get; set; }
        public decimal ThrowRange { get; set; }
    
        public short[] BaseType { get; set; } = new short[2]; // Base item Type of type ItemEffectInstant or ItemEffectPassive
        public decimal[,] BaseVar { get; set; } = new decimal[2, 4]; 
        public short[] OptType { get; set; } = new short[2]; // Extends Base item type by ItemEffectInstant or ItemEffectPassive
        public decimal[,] OptVar { get; set; } = new decimal[2,4];

        public int[] EnchanceId { get; set; } = new int[2];
        public decimal[,] EnchanceVar { get; set; } = new decimal[2,4];

        public int StateLevel { get; set; }
        public int StateTime { get; set; }
        public int CoolTime { get; set; }
        public short CoolTimeGroup { get; set; } // could not find related enum
        public string ScriptText { get; set; }
    
        public int NameId { get; set; } 
    
        public int TooltipId { get; set; }  
        // public virtual StringResourceEntity Tooltip { get; set; }
    
        public int SetId { get; set; } 
        // public virtual SetItemEffectResourceEntity Set { get; set; }
    
        public int SummonId { get; set; } 
        // public virtual SummonResourceEntity Summon { get; set; }
    
        public int EffectId { get; set; }
        // public virtual EffectResourceEntity Effect { get; set; }
    
        public int SkillId { get; set; }  
        // public virtual SkillResourceEntity Skill { get; set; }
    
        public int StateId { get; set; }
        // public virtual StateResourceEntity State { get; set; }
    
        // public virtual EnhanceResourceEntity[] Enchance { get; set; }
    }
}
