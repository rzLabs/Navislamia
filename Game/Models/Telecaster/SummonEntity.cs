using Navislamia.Game.Models.Arcadia;

namespace Navislamia.Game.Models.Telecaster;

public class SummonEntity: Entity
{

    // relation not possible since you can't create a realation cross multiple contexts
    // (TelecasterContext => AuthContext)
    public int AccountId {get;set;}
    
    public int CharacterId {get;set;}
    public virtual CharacterEntity Character { get; set; }
    
    public int Code {get;set;}
    
    public long CardItemId {get;set;}
    public virtual ItemEntity CardItem { get; set; }

    public long Exp {get;set;}
    public int Jp {get;set;}
    public long LastDecreasedExp {get;set;}
    public string Name {get;set;}
    public int Transform {get;set;}
    public int Lv {get;set;}
    public int Jlv {get;set;}
    public int MaxLevel {get;set;}
    public int Fp {get;set;}
    public int PrevLevel01 {get;set;}
    public int PrevLevel02 {get;set;}
    public int PrevId01 {get;set;}
    public int PrevId02 {get;set;}
    public int Sp {get;set;}
    public int Hp {get;set;}
    public int Mp {get;set;}
}
