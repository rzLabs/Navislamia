using System.Collections.Generic;

namespace Navislamia.Game.Models.Arcadia;

public class ModelEffectResourceEntity : Entity
{
    public long? EffectFileId {get;set;}
    public virtual EffectResourceEntity EffectFile { get; set; }
    
    public bool LoopEffect {get;set;} // was int previously with values 0 and 1 check usage and adjust
    public int? EffectPosition {get;set;} // 0 1 or 3 -> enum? check usage and adjust
    public string?[]? BoneNames { get; set; } // = new string[15];
    public long?[]? BoneEffectIds { get; set; } // = new int[15];
    
    public virtual ICollection<EffectResourceEntity> BoneEffects { get; set; }
    public virtual ICollection<SummonResourceEntity> SummonModels { get; set; }
    
    // public int SwapEventId {get;set;} // always 0 removed for now
    // public int SwapModelId {get;set;} // always 0 removed for now
}