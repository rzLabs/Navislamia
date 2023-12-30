using System;

namespace Navislamia.Game.DataAccess.Entities.Enums
{
    [Flags]
    public enum ItemUseFlag
    {
        CantDonate = 0,
        CantStorage = 1,
        CantEnhance = 2,
        Use = 3,
        Card = 4,
        Socket = 5,
        Join = 6,
        TargetUse = 7,
        Warp = 8,
        CantTrade = 9,
        CantSell = 10,
        Quest = 11,
        CantUseOverweight = 12,
        Cashitem = 13,
        CantUseRiding = 14,
        CantDrop = 15,
        CantUseMoving = 16,
        QuestDistribute = 17,
        CantUseSit = 18,
        CantUseInRaidSiege = 19,
        CantUseInSecroute = 20,
        CantUseInEventmap = 21,
        CantUseInHuntaholic = 22,
        UsableInOnlyHuntaholic = 23,
        CantUseInDeathmatch = 24,
        UsableInOnlyDeathmatch = 25,
        NotErasable = 26
    }
}