using System;

namespace Navislamia.Game.Models.Enums
{
    [Flags]
    public enum StateTimeType : short
    {
        EraseOnDead = 1,
        EraseOnLogout = 2,
        TimeDecreaseOnLogout = 4,

        NotActableToBoss = 8,
        NotErasable = 16,
        EraseOnRequest = 32,
        EraseOnDamaged = 64,
        EraseOnResurrect = 128,
        EraseOnQuitHuntaholic = 256,

        EraseOnQuitDeathmatch = 512,
        NotErasableOnEnterDeathmatch = 1024,
        NotActableOnDeathmatch = 2048,

        EraseOnCompeteStart = 4096,
        NotActableInCompete = 8192,
    }
}