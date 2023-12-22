using Navislamia.Game.Models.Arcadia.Enums;
using Navislamia.Game.Models.Enums;

namespace Navislamia.Game.Models.Arcadia
{
    public class EnhanceResourceEntity : Entity
    {
        public EnhanceType EnhanceType { get; set; }
        public FailResultType FailResult { get; set; }
        public LocalFlag LocalFlag { get; set; }
        public int RequiredItemId { get; set; }
        public short MaxEnhance { get; set; }
    
        public decimal[] Percentage { get; set; } = new decimal[20]; 

    }
}