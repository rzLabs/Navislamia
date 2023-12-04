using System;

namespace DevConsole.Models.Arcadia.Enums;

[Flags]
public enum ItemStatus
{
    ItemFlagCard = 0,
    ItemFlagFull = 1,
    ItemFlagInserted = 2,
    ItemFlagFailed = 3,
    ItemFlagEvent = 4,
    ItemFlagContainPet = 5,
    ItemFlagSummonDurability = 6,
    ItemFlagFarmedSummon = 27,
    ItemFlagNursedSummon = 28,
    ItemFlagTaming = 29,
    ItemFlagNonChaosStone = 30,
    ItemFlagSummon = 31 
}