namespace Navislamia.Game.Models.Arcadia
{
    public class EffectResourceEntity : Entity
    {
        public string FileName { get; set; }
    
        public virtual ItemResourceEntity Item { get; set; }
        public virtual SetItemEffectResourceEntity Set { get; set; }
        public virtual ModelEffectResourceEntity Model { get; set; }
        public virtual ItemEffectResourceEntity ItemEffect { get; set; }
    }
}