using Navislamia.Game.Models.Arcadia.Enums;

namespace Navislamia.Game.Models.Arcadia
{
    public class EnhanceResourceEntity : Entity
    {
        public EnhanceType EnhanceType { get; set; }
        public FailResultType FailResult { get; set; }
        public int LocalFlag { get; set; }
        public int NeedItem { get; set; }
        public short MaxEnhance { get; set; }
    
        public decimal[] Percentage { get; set; } = new decimal[20]; 

    }
}