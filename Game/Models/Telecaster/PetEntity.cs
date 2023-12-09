using Navislamia.Game.Models.Arcadia;

namespace Navislamia.Game.Models.Telecaster;

public class PetEntity : Entity
{
    // relation not possible since you can't create a relation across contexts
    // (TelecasterContext => AuthContext)
    public int AccountId { get; set; }

    public int CharacterId { get; set; }
    public virtual CharacterEntity Character { get; set; }
    
    public long ItemId { get; set; } // previously cage_uid
    public virtual ItemEntity Item { get; set; }
    
    public int Code { get; set; } // unused?
    public string Name { get; set; }
    public bool NameChanged { get; set; }
    public int CoolTime { get; set; } // convert to datetime? check usage
}