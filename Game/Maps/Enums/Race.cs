using System;

namespace Navislamia.Game.Maps.Enums;

[Flags]
public enum Race
{
    Gaia = 1 << 0,
    Deva = 1 << 1,
    Asura = 1 << 2,

    All = Gaia | Deva | Asura
}