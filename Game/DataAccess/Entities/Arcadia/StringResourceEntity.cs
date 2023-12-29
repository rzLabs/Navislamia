using System.Collections.Generic;

namespace Navislamia.Game.DataAccess.Entities.Arcadia;

public class StringResourceEntity : Entity
{
    public string Name { get; set; }
    public string Value { get; set; }

    // These navigational properties are not really required for this entity
    // but can be loaded when loading the respective entity. Cull by usage in the future
    public virtual ICollection<ItemResourceEntity> ItemResourceNames { get; set; }
    public virtual ICollection<ItemResourceEntity> ItemResourceTooltips { get; set; }
    
    public virtual ICollection<ItemEffectResourceEntity> ItemEffectToolTips { get; set; }
    
    public virtual ICollection<SetItemEffectResourceEntity> SetTexts { get; set; }
    public virtual ICollection<SetItemEffectResourceEntity> SetTooltips { get; set; }
    
    public virtual ICollection<SummonResourceEntity> SummonSkillTexts { get; set; }
    public virtual ICollection<SummonResourceEntity> SummonNames { get; set; }
    
    public virtual ICollection<SkillResourceEntity> SkillTexts { get; set; }
    public virtual ICollection<SkillResourceEntity> SkillDescriptions { get; set; }
    public virtual ICollection<SkillResourceEntity> SkillTooltips { get; set; }
    
    public virtual ICollection<StateResourceEntity> StateTexts { get; set; }
    public virtual ICollection<StateResourceEntity> StateTooltips { get; set; }
}
