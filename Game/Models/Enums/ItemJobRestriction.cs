using System;

namespace Navislamia.Game.Models.Enums;

[Flags]
public enum ItemJobRestriction
{
    None = 0,
    Fighter = 1024,
    Hunter = 2048,
    Magician = 4096,
    Summoner = 8192
}
