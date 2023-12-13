using Navislamia.Game.Models.Arcadia.Enums;
using Navislamia.Game.Models.Enums;

namespace Navislamia.Game.Models.Arcadia
{
	public class SummonResourceEntity : Entity
	{
		public int ModelId { get; set; }
	
		public CreatureType Type { get; set; }
		public ElementalType MagicType { get; set; }
	
		public decimal Size { get; set; } 
		public decimal Scale { get; set; } 
		public decimal TargetFxSize { get; set; } 
	
		public int StandardWalkSpeed { get; set; }
		public int StandardRunSpeed { get; set; }
		public bool IsRidingOnly { get; set; }
		public int RidingSpeed { get; set; }
		public int RunSpeed { get; set; }
		public int RidingMotionType { get; set; }
		public decimal AttackRange { get; set; }
	
		public int WalkType { get; set; } // check enum from rdb usage (e.g. angel = 34 probably flying)
		public int SlantType { get; set; } // 0 or 1 check usage
		public int Material { get; set; }  // check enum from rdb (values 0 - 6)
		public ItemType WeaponType { get; set; } 
		public int AttackMotionSpeed { get; set; } // check enum from rdb (values 0 - 3)
		public EvolveType EvolveType { get; set; } 
		public int EvolveIntoSummonId { get; set; }
		// public virtual SummonResourceEntity EvolveTarget { get; set; }
	
		public int[] CameraPosition { get; set; } // X Y Z 
	
		public decimal[] TargetPosition { get; set; } // X Y Z
		public string Model { get; set; }
	
		public int MotionFileId { get; set; }
		public int FaceId { get; set; }
		public string FaceFileName { get; set; }
		public int CardId { get; set; }
		public string ScriptText { get; set; }
		public string IllustFileName { get; set; }
	
		public int TextFeatureId { get; set; } 

		public int[] SkillIds { get; set; } // 5
		public int[] SkillTextIds { get; set; } // 5

		public int StatId { get; set; }
		public int NameId { get; set; }
		public int TextureGroup { get; set; } 

		// public virtual StringResourceEntity Script { get; set; }
		// public virtual ItemResourceEntity Card { get; set; }
		// public virtual StringResourceEntity TextFeature { get; set; }
		// public virtual StringResourceEntity Skill4Text { get; set; }
		// public virtual StringResourceEntity Skill3Text { get; set; }
		// public virtual StringResourceEntity Skill2Text { get; set; }
		// public virtual StatResourceEntity Stat { get; set; }
		// public virtual StringResourceEntity Name { get; set; }
		// public virtual StringResourceEntity Skill1Text { get; set; }
		// public virtual StringResourceEntity Skill5Text { get; set; }
	}
}
