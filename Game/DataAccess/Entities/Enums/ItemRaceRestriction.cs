using System;

namespace Navislamia.Game.DataAccess.Entities.Enums;

[Flags]
public enum ItemRaceRestriction
{
    None = 0,
    Deva = 1,
    Asura = 2,
    Gaia = 4,
}
