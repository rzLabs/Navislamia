using System.Collections.Generic;

namespace Navislamia.Game.DataAccess.Entities.Arcadia;

public class StatResourceEntity : Entity
{
    public int Strength { get; set; }
    public int Vitality { get; set; }
    public int Dexterity { get; set; }
    public int Agility { get; set; }
    public int Intelligence { get; set; }
    public int Wisdom { get; set; }
    public int Luck { get; set; }
    
    public virtual ICollection<SummonResourceEntity> Summons { get; set; }
}
