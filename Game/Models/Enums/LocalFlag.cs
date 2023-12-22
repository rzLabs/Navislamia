using System;

namespace Navislamia.Game.Models.Enums;

[Flags]
public enum LocalFlag
{
    Korea = 1 << 0,
    Hongkong = 1 << 1,
    America = 1 << 2,
    German = 1 << 3,
    Japan = 1 << 4,
    Taiwan = 1 << 5,
    China = 1 << 6,
    France = 1 << 7,
    Russia = 1 << 8,
    Malaysia = 1 << 9,
    Singapore = 1 << 10,
    Vietnam = 1 << 11,
    Thailand = 1 << 12,
    Mideast = 1 << 13,
    Turkey = 1 << 14,
    Poland = 1 << 15,
    Italy = 1 << 16,

    ExcludeTestServ = 1 << 29,
    ExcludeServiceServ = 1 << 30
}