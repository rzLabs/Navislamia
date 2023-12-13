using System;

namespace Navislamia.Game.Models.Arcadia.Enums
{
    [Flags]
    public enum SetPartFlag : short
    {
        None = 0,
        Weapon = 1,
        Shield = 2,
        Armor = 4,
        Helmet = 8,
        Glove = 16,
        Boots = 32
    }
}