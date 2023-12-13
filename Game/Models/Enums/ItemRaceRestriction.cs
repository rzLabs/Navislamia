using System;

namespace Navislamia.Game.Models.Enums;

[Flags]
public enum ItemRaceRestriction
{
    None = 0,
    Deva = 1,
    Asura = 2,
    Gaia = 4,
}
