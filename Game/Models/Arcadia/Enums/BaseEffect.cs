namespace DevConsole.Models.Arcadia.Enums;

public enum StateBaseEffect
{
    None = 0,
    PhysicalStateDamage = 1,
    PhysicalIgnoreDenceStateDamage = 2,
    MagicalStateDamage = 3,
    MagicalIgnoreResistStateDamage = 4,
    
    PhysicalIgnoreDencePerStateDamage = 6,

    HealHpByMagic = 11,
    HealMpByMagic = 12,
    HealSpByMagic = 13,
    HealHpByItem = 21,
    HealMpByItem = 22,
    HealSpByItem = 23,
    HealHpmpByItem = 24,
    HealHpmpPerByItem = 25,

    Poison = 51,
    Venom = 52,
    Bloody = 53,
    SeriousBloody = 54,
};