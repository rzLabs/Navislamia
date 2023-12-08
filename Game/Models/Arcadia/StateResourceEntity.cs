using Navislamia.Game.Models.Arcadia.Enums;

namespace Navislamia.Game.Models.Arcadia
{
	public class StateResourceEntity : Entity
	{
		public int TextId { get; set; }
		public int TooltipId { get; set; }
		public bool IsHarmful { get; set; }
		public StateTimeType StateTimeType { get; set; }
		public StateGroup StateGroup { get; set; }
		public int[] DuplicateGroup { get; set; } 
		public string UfAvatar { get; set; }
		public string UfSummon { get; set; }
		public string UfMonster { get; set; }
		public string ReiterationCount { get; set; }
		public StateBaseEffect BaseEffect { get; set; }
		public int FireInterval { get; set; }
		public ElementalType ElementalType { get; set; }
		public decimal AmplifyBase { get; set; } 
		public decimal AmplifyPerSkill { get; set; }
		public int AddDamageBase { get; set; }
		public int AddDamagePerSkl { get; set; }
		public SkillEffectType EffectType { get; set; }
		public int[] Value { get; set; } = new int[20];
		public int IconId { get; set; }
		public string IconFileName { get; set; }
		public int FxId { get; set; }
		public int PosId { get; set; }
		public int CastSkillId { get; set; }
		public int CastFxId { get; set; }
		public int CastFxPosId { get; set; }
		public int HitFxId { get; set; }
		public int HitFxPosId { get; set; }
		public int SpecialOutputTimingId { get; set; }
		public int SpecialOutputFxId { get; set; }
		public int SpecialOutputFxPosId { get; set; }
		public int SpecialOutputFxDelay { get; set; }
		public int StateFxId { get; set; }
		public int StateFxPosId { get; set; }
	
		// public virtual ItemResourceEntity ItemResource { get; set; }
		// public virtual StringResourceEntity Text { get; set; }
		// public virtual StringResourceEntity Tooltip { get; set; }

	}
}
