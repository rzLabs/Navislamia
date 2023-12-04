using DevConsole.Models.Arcadia.Enums;

namespace DevConsole.Models.Arcadia;

public class SkillResourceEntity : Entity
{
	public int TextId { get; set; }
	public int DescId { get; set; }
	public int TooltipId { get; set; }
	
	public string Elemental { get; set; }
	public string IsPassive { get; set; }
	public string IsPhysicalAct { get; set; }
	public string IsHarmful { get; set; }
	public string IsNeedTarget { get; set; }
	public string IsCorpse { get; set; }
	public string IsToggle { get; set; }
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
	
	public int NeedLevel { get; set; }
	public int NeedHp { get; set; }
	public int NeedMp { get; set; }
	public int NeedHavoc { get; set; }
	public int NeedHavocBurst { get; set; }
	public int NeedStateId { get; set; }
	
	public string VfOneHandSword { get; set; }
	public string VfTwoHandSword { get; set; }
	public string VfDoubleSword { get; set; }
	public string VfDagger { get; set; }
	public string VfDoubleDagger { get; set; }
	public string VfSpear { get; set; }
	public string VfAxe { get; set; }
	public string VfOneHandAxe { get; set; }
	public string VfDoubleAxe { get; set; }
	public string VfOneHandMace { get; set; }
	public string VfTwoHandMace { get; set; }
	public string VfLightbow { get; set; }
	public string VfHeavybow { get; set; }
	public string VfCrossbow { get; set; }
	public string VfOneHandStaff { get; set; }
	public string VfTwoHandStaff { get; set; }
	public string VfShieldOnly { get; set; }
	public string VfIsNotNeedWeapon { get; set; }
	
	public decimal DelayCast { get; set; } 
	public decimal DelayCastPerSkl { get; set; } 
	public decimal DelayCastModePerEnhance { get; set; } 
	public decimal DelayCommon { get; set; } 
	public decimal DelayCooltime { get; set; } 
	public decimal DelayCooltimePerSkl { get; set; } 
	public decimal DelayCooltimeModePerEnhance { get; set; } 
	public int CoolTimeGroupId { get; set; }
	
	public string UfSelf { get; set; }
	public string UfParty { get; set; }
	public string UfGuild { get; set; }
	public string UfNeutral { get; set; }
	public string UfPurple { get; set; }
	public string UfEnemy { get; set; }
	
	public string TfAvatar { get; set; }
	public string TfSummon { get; set; }
	public string TfMonster { get; set; }
	
	public string SkillLvupLimit { get; set; }
	public SkillTargetType Target { get; set; }
	public SkillEffectType EffectType { get; set; }
	public int SkillEnchantLinkId { get; set; }
	public int StateId { get; set; }
	public int StateLevelBase { get; set; }
	
	public decimal StateLevelPerSkl { get; set; } 
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
	public decimal[] Var { get; set; } = new decimal[20]; 
	
	public int IconId { get; set; }
	public string IconFileName { get; set; }
	
	public decimal ProjectileSpeed { get; set; } 
	public decimal ProjectileAcceleration { get; set; } 
	
	// public virtual StateResourceEntity NeedStat { get; set; }
	// public virtual StateResourceEntity State { get; set; }
	// public virtual ItemResourceEntity ItemResources { get; set; }
	// public virtual StringResourceEntity Text { get; set; }
	// public virtual StringResourceEntity Desc { get; set; }
	// public virtual StringResourceEntity Tooltip { get; set; }


	
}
