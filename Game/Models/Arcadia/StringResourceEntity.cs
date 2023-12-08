namespace Navislamia.Game.Models.Arcadia
{
    public class StringResourceEntity : Entity
    {
        // TODO Id used to be named "code" remove comment once Stage-1 is completed
        public string Name { get; set; }
        public string Value { get; set; }

        // public virtual ItemResourceEntity ItemResourceName { get; set; }
        // public virtual ItemResourceEntity ItemResourceTooltip { get; set; }
        // public virtual ItemEffectResourceEntity ItemEffectToolTip { get; set; }
        // public virtual SetItemEffectResourceEntity SetItemEffectResourceText { get; set; }
        // public virtual SetItemEffectResourceEntity SetItemEffectResourceTooltip { get; set; }
    }
}