namespace DevConsole.Models.Arcadia.Enums;

/// <summary>
/// Is the same as <see cref="SetPartFlag"/> but not flaggable
/// ItemResource uses one type per Item and SetItemEffectResource uses multiple items (part flags) per set effect
/// </summary>

// TODO Unit tests to ensure PartFlags and PartTypes cover each other completely
public enum SetPartType
{
    None = 0,
    Weapon = 1,
    Shield = 2,
    Armor = 4,
    Helmet = 8,
    Glove = 16,
    Boots = 32
}