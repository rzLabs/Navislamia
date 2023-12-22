using System.Collections.Generic;
using Navislamia.Game.Models.Arcadia.Enums;
using Navislamia.Game.Models.Enums;

namespace Navislamia.Game.Models.Arcadia;

public class SkillResourceEntity : Entity
{
	public long? TextId { get; set; }
	public virtual StringResourceEntity Text { get; set; }
	
	public long? DescriptionId { get; set; }
	public virtual StringResourceEntity Description { get; set; }
	
	public long? TooltipId { get; set; }
	public virtual StringResourceEntity Tooltip { get; set; }
	
	public ElementalType ElementalType { get; set; }
	public bool IsPassive { get; set; }
	public SkillState IsValid { get; set; }
	public bool IsPhysicalAct { get; set; }
	public bool IsHarmful { get; set; }
	public SkillRequiredTarget RequiredTarget { get; set; } 
	public bool IsCorpse { get; set; }
	public bool IsToggle { get; set; }
	public bool IsProjectile { get; set; }
	
	// groups where only one skill in the group can be active
	// e.g. Group 8 Unity of Force, Intelligence, Wisdom, Mentality, Vitality
	public int ToggleGroup { get; set; } 

	public string CastingType { get; set; }
	public string CastingLevel { get; set; }
	public int CastRange { get; set; }
	public int ValidRange { get; set; }

	public int CostHp { get; set; }
	public int CostHpPerSkl { get; set; }
	public int CostMp { get; set; }
	public int CostMpPerSkl { get; set; }
	public int CostMpPerEnhance { get; set; }
	public decimal CostHpPer { get; set; } 
	public decimal CostHpPerSklPer { get; set; } 
	public decimal CostMpPer { get; set; }
	public decimal CostMpPerSklPer { get; set; } 
	public int CostHavoc { get; set; }
	public int CostHavocPerSkl { get; set; }
	public decimal CostEnergy { get; set; }
	public decimal CostEnergyPerSkl { get; set; }
	public int CostExp { get; set; }
	public int CostExpPerEnhance { get; set; }
	public int CostJp { get; set; }
	public int CostJpPerEnhance { get; set; }
	public int CostItem { get; set; }
	public int CostItemCount { get; set; }
	public int CostItemCountPerSkl { get; set; }

	public int RequiredLevel { get; set; }
	public int RequiredHp { get; set; }
	public int RequiredMp { get; set; }
	public int RequiredHavoc { get; set; }
	public int RequiredHavocBurst { get; set; }
	public long? RequiredStateId { get; set; }
	public virtual StateResourceEntity RequiredState { get; set; }

	public bool UseWithOneHandSword { get; set; }
	public bool UseWithTwoHandSword { get; set; }
	public bool UseWithDoubleSword { get; set; }
	public bool UseWithDagger { get; set; }
	public bool UseWithDoubleDagger { get; set; }
	public bool UseWithSpear { get; set; }
	public bool UseWithAxe { get; set; }
	public bool UseWithOneHandAxe { get; set; }
	public bool UseWithDoubleAxe { get; set; }
	public bool UseWithOneHandMace { get; set; }
	public bool UseWithTwoHandMace { get; set; }
	public bool UseWithLightbow { get; set; }
	public bool UseWithHeavybow { get; set; }
	public bool UseWithCrossbow { get; set; }
	public bool UseWithOneHandStaff { get; set; }
	public bool UseWithTwoHandStaff { get; set; }
	public bool UseWithShieldOnly { get; set; }
	public bool UseWithWeaponNotRequired { get; set; }
	public bool UseOnSelf { get; set; }
	public bool UseOnParty { get; set; }
	public bool UseOnGuild { get; set; }
	public bool UseOnNeutral { get; set; }
	public bool UseOnPurple { get; set; }
	public bool UseOnEnemy { get; set; }
	public bool UseOnCharacter { get; set; }
	public bool UseOnSummon { get; set; }
	public bool UseOnMonster { get; set; }

	public decimal DelayCast { get; set; } 
	public decimal DelayCastPerSkl { get; set; } 
	public decimal DelayCastModePerEnhance { get; set; } 
	public decimal DelayCommon { get; set; } 
	public decimal DelayCooltime { get; set; } 
	public decimal DelayCooltimePerSkl { get; set; } 
	public decimal DelayCooltimeModePerEnhance { get; set; } 
	public int CoolTimeGroupId { get; set; }


	public string SkillLvupLimit { get; set; }
	public SkillTarget Target { get; set; }
	public SkillEffectType EffectType { get; set; }
	public long? UpgradeIntoSkillId { get; set; }
	public virtual SkillResourceEntity SkillUpgrade { get; set; }
	public virtual SkillResourceEntity Skill { get; set; }
	public long? StateId { get; set; }
	public virtual StateResourceEntity State { get; set; }
	public int StateLevelBase { get; set; }

	public decimal StateLevelPerSkill { get; set; } 
	public decimal StateLevelPerEnhance { get; set; } 
	public decimal StateSecond { get; set; } 
	public decimal StateSecondPerLevel { get; set; } 
	public decimal StateSecondPerEnhance { get; set; } 

	public int ProbabilityOnHit { get; set; }
	public int ProbabilityIncBySlv { get; set; }

	public short HitBonus { get; set; }
	public short HitBonusPerEnhance { get; set; }

	public short Percentage { get; set; }

	public decimal HateMod { get; set; } 
	public short HateBasic { get; set; }
	public decimal HatePerSkill { get; set; } 
	public decimal HatePerEnhance { get; set; } 

	public int CriticalBonus { get; set; }
	public int CriticalBonusPerSkl { get; set; }
	public decimal[] Values { get; set; } // = new decimal[20]; 

	public long? IconId { get; set; }
	public string IconFileName { get; set; }

	public decimal ProjectileSpeed { get; set; } 
	public decimal ProjectileAcceleration { get; set; } 

	public virtual ICollection<ItemResourceEntity> Items { get; set; }
	public virtual SummonResourceEntity Summon { get; set; }
}
