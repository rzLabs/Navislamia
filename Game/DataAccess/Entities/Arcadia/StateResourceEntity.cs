using System.Collections.Generic;
using Navislamia.Game.DataAccess.Entities.Enums;

namespace Navislamia.Game.DataAccess.Entities.Arcadia;

public class StateResourceEntity : Entity
{
	public long? TextId { get; set; }
	public virtual StringResourceEntity Text { get; set; }
	
	public long? TooltipId { get; set; }
	public virtual StringResourceEntity Tooltip { get; set; }
	
	public StateTimeType StateTimeType { get; set; }
	public StateGroup StateGroup { get; set; }
	public ElementalType ElementalType { get; set; }
	public SkillEffectType EffectType { get; set; }
	public StateBaseEffect BaseEffect { get; set; }
	
	public bool IsHarmful { get; set; }
	public bool UseOnCharacter { get; set; }
	public bool UseOnSummon { get; set; }
	public bool UseOnMonster { get; set; }
	public int[] DuplicateGroup { get; set; } 
	public string ReiterationCount { get; set; }
	public int FireInterval { get; set; } // is this supposed to be cast/shoot speed of skills/arrows?
	public decimal AmplifyBase { get; set; } 
	public decimal AmplifyPerSkill { get; set; }
	public int AddDamageBase { get; set; }
	public int AddDamagePerSkl { get; set; }
	public decimal[] Values { get; set; } = new decimal[20];
	public long IconId { get; set; }
	public string IconFileName { get; set; }
	public long FxId { get; set; } // what entity is this?
	public long PosId { get; set; }
	public int SpecialOutputTimingId {  get; set; }
	public int SpecialOutputFxId { get; set; }
	public int SpecialOutputFxPosId { get; set; }

	public virtual ICollection<ItemResourceEntity> Items { get; set; }
	public virtual ICollection<SkillResourceEntity> Skills { get; set; }
	public virtual ICollection<SkillResourceEntity> RequiredBySkills { get; set; }

	// public int SpecialOutputFxDelay { get; set; } // removed always 0
	// public int StateFxId { get; set; } // removed always 0
	// public int StateFxPosId { get; set; } // removed always 0
	// public int CastSkillId { get; set; } // removed always 0
	// public int CastFxId { get; set; } // removed always 0
	// public int CastFxPosId { get; set; } // removed always 0
	// public int HitFxId { get; set; } // removed always 0
	// public int HitFxPosId { get; set; } // removed always 0
}
