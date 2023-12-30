namespace Navislamia.Game.DataAccess.Entities.Telecaster;

public class SummonEntity : Entity
{
    // relation not possible since you can't create a realation cross multiple contexts
    // (TelecasterContext => AuthContext)
    public long AccountId { get; set; }

    public long CharacterId { get; set; }
    public virtual CharacterEntity MainSummonsMaster { get; set; }
    public virtual CharacterEntity SubSummonsMaster { get; set; }

    public int SummonResourceId { get; set; }

    public long CardItemId { get; set; }
    public virtual ItemEntity CardItem { get; set; }

    public long Exp { get; set; }
    public int Jp { get; set; }
    public long LastDecreasedExp { get; set; }
    public string Name { get; set; }
    public int Transform { get; set; }
    public int Lv { get; set; }
    public int Jlv { get; set; }
    public int MaxLevel { get; set; }
    public int Fp { get; set; }
    public int[] PreviousLevel { get; set; } 
    public long[] PreviousSummonResourceIds { get; set; } 
    
    public int Sp { get; set; } // check usage and refactor if (not)requried
    public int Hp { get; set; }
    public int Mp { get; set; }
}