namespace Navislamia.Game.DataAccess.Entities.Telecaster;

public class PetEntity : Entity
{
    // relation not possible since you can't create a relation across contexts
    // (TelecasterContext => AuthContext)
    public long AccountId { get; set; }

    public long CharacterId { get; set; }
    public virtual CharacterEntity Character { get; set; }
    
    public long ItemId { get; set; }
    public virtual ItemEntity Item { get; set; }
    
    public int PetResourceId { get; set; }
    public string Name { get; set; }
    public bool WasNameChanged { get; set; }
    public int CoolTime { get; set; } // might be unused check and remove if unused
}