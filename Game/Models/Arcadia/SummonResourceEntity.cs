using Navislamia.Game.Models.Arcadia.Enums;
using Navislamia.Game.Models.Enums;

namespace Navislamia.Game.Models.Arcadia
{
	public class SummonResourceEntity : Entity
	{
		public int ModelId { get; set; } // RDB?
	
		public CreatureType Type { get; set; }
		public ElementalType MagicType { get; set; }
	
		public decimal Size { get; set; } 
		public decimal Scale { get; set; } 
		public decimal TargetFxSize { get; set; } 
	
		public int StandardWalkSpeed { get; set; }
		public int StandardRunSpeed { get; set; }
		public int IsRidingOnly { get; set; }
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
		public int EvolveTargetId { get; set; } // Evolve into <SummonID>
		// public virtual SummonResourceEntity EvolveTarget { get; set; }
	
		public int CameraX { get; set; }
		public int CameraY { get; set; }
		public int CameraZ { get; set; }
	
		public decimal TargetX { get; set; } 
		public decimal TargetY { get; set; } 
		public decimal TargetZ { get; set; }
		public string Model { get; set; }
	
		public int MotionFileId { get; set; }
		public int FaceId { get; set; }
		public string FaceFileName { get; set; }
		public int CardId { get; set; }
	
		public string ScriptText { get; set; }

		public string IllustFileName { get; set; }
	
		public int TextFeatureId { get; set; } // check usage, maybe Tooltip or Description would be a better name
		public int TextNameId { get; set; } // unused? Is always 0 in SummonResource

		public int Skill1Id { get; set; }
		public int Skill1TextId { get; set; }
		public int Skill2Id { get; set; }
		public int Skill2TextId { get; set; }
		public int Skill3Id { get; set; }
		public int Skill3TextId { get; set; }
		public int Skill4Id { get; set; }
		public int Skill4TextId { get; set; }
		public int Skill5Id { get; set; }
		public int Skill5TextId { get; set; }

		public int StatId { get; set; }
		public int NameId { get; set; }
		public int TextureGroup { get; set; } // unused? all entities are set to -1 in SummonResource
		public int LocalFlag { get; set; } // unused? all entities are set to 0 in SummonResource
	

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
