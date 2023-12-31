using System.Collections.Generic;
using Navislamia.Game.DataAccess.Entities.Enums;

namespace Navislamia.Game.DataAccess.Entities.Arcadia;

public class SummonResourceEntity : Entity
{
	public CreatureType Type { get; set; }
	public ItemType WeaponType { get; set; } 
	public ElementalType MagicType { get; set; }
	public EvolveType EvolveType { get; set; } 
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
	public int AttackMotionSpeed { get; set; } // check enum from rdb (values 0 - 3)
	
	public long? EvolveTargetId { get; set; }
	public virtual SummonResourceEntity EvolveTarget { get; set; }
	public virtual SummonResourceEntity EvolveSource { get; set; }

	public int[] CameraPosition { get; set; } // X Y Z 
	public decimal[] TargetPosition { get; set; } // X Y Z
	public string ModelName { get; set; }
	public long MotionFileId { get; set; }
	public long FaceId { get; set; }
	public string FaceFileName { get; set; }
	public string IllustFileName { get; set; }
	public long TextFeatureId { get; set; } // is -1 or 0 check usage and adjust/remove

	public long? CardId { get; set; }
	public virtual ItemResourceEntity Card { get; set; }
	
	public long?[]? SkillIds { get; set; } // 5
	public virtual ICollection<SkillResourceEntity> Skills { get; set; }
	
	// only 3 entities use those, but the id does not exist in the StringResource anymore
	// public long?[]? SkillTextIds { get; set; } // 5
	// public virtual ICollection<StringResourceEntity> SkillTexts { get; set; }

	public long? StatId { get; set; }
	public virtual StatResourceEntity Stat { get; set; }
	
	public long? NameId { get; set; }
	public virtual StringResourceEntity Name { get; set; }
	
	public int TextureGroup { get; set; } 
	
	public long? ModelId { get; set; }
	public virtual ModelEffectResourceEntity Model { get; set; }
	
	public virtual ICollection<ItemResourceEntity> Items { get; set; }
	
	// public string ScriptText { get; set; } // removed always 0
}
