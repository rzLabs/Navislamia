namespace Navislamia.Game.DataAccess.Entities.Enums;

public enum SkillState
{
    Invalid = 0,
    Valid = 1,
    System = 2, 
    
    // (refactor to use boolean instead of this enum)
    // refactor to just use Invalid/Valid as originaly IsValid is true when Valid or System
}