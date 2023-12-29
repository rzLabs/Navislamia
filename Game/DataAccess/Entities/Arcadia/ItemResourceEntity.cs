using System.Collections.Generic;
using Navislamia.Game.DataAccess.Entities.Enums;
using ItemGroup = Navislamia.Game.DataAccess.Entities.Enums.ItemGroup;
using ItemType = Navislamia.Game.DataAccess.Entities.Enums.ItemType;
using ItemWearType = Navislamia.Game.DataAccess.Entities.Enums.ItemWearType;

namespace Navislamia.Game.DataAccess.Entities.Arcadia;

public class ItemResourceEntity : Entity
{
    public ItemBaseType ItemBaseType { get; set; } 
    public ItemType ItemType { get; set; } 
    public ItemGroup Group { get; set; } 
    public ItemWearType WearType { get; set; } 
    public SetParts SetPart { get; set; } 
    public ItemStatus Status { get; set; }
    public ItemRaceRestriction RaceRestriction { get; set; }
    public ItemJobRestriction JobRestriction { get; set; }
    public ItemUseFlag ItemUseFlag { get; set; }
    public ItemDecreaseTimeType DecreaseType { get; set; }

    public int Grade { get; set; } // 0 - 7 -> how much dura is consumed when attacking/being attacked -> enum?
    public int Rank { get; set; }
    public int Level { get; set; }
    public int Enhance { get; set; } 
    public int SocketCount { get; set; }
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

    public short[] BaseTypes { get; set; } // = new short[4]; // Base item Type of type ItemEffectInstant or ItemEffectPassive
    public decimal[,] BaseValues { get; set; } // = new decimal[4, 2];
    
    public short[] OptTypes { get; set; } // = new short[4]; // Extends Base item type by ItemEffectInstant or ItemEffectPassive
    public decimal[,] OptValues { get; set; } // = new decimal[4,2];

    // -> Might not work with "Enhances" as a navigational property check and confirm.
    // -> It will definetly work by splitting the id array into seperate ids and creating a realtion to each of them
    /// <summary>
    /// This is a nullable array with nullable values. <para/>
    /// 
    /// This means [null, 1234] is possible as is [1234]
    /// Take care when translating to set the Id into the correct place. <para/>
    /// 
    /// On the other hand it might not be a problem as if only one Id exists, you use only one <para/>
    /// 
    /// Permutations: [] [123] [123, 456] [null, 123] [123, null] where last one most likely will be visible as [123] unless you explicitly set the second it to null
    /// </summary>
    public long[]? EnhanceIds { get; set; } // = new int[2]; 
    public decimal[,] EnhanceValues { get; set; } // = new decimal[2,4];

    public int StateLevel { get; set; }
    public int StateTime { get; set; }
    public int CoolTime { get; set; }
    public short CoolTimeGroup { get; set; } // Groupid of items sharing a cooldown timer could be refractored into an enum -> referr to captain or usage later on
    public string? ScriptText { get; set; }

    public long? NameId { get; set; } 
    public virtual StringResourceEntity Name { get; set; }

    public long? TooltipId { get; set; }  
    public virtual StringResourceEntity Tooltip { get; set; }

    public long? SetId { get; set; } 
    // removed relation since its not convertible with the old login in place. Try again at a later stage
    // public SetParts SetParts { get; set; } 
    // public virtual SetItemEffectResourceEntity Set { get; set; }

    public long? SummonId { get; set; } 
    public virtual SummonResourceEntity Summon { get; set; }

    public long? EffectId { get; set; }
    public virtual EffectResourceEntity Effect { get; set; }

    public long? SkillId { get; set; }  
    public virtual SkillResourceEntity Skill { get; set; }

    public long? StateId { get; set; }
    public virtual StateResourceEntity State { get; set; }
    
    public virtual ICollection<EnhanceResourceEntity> RequiredByEnhanceResources { get; set; }
    public virtual ICollection<SummonResourceEntity> Cards { get; set; }
}
