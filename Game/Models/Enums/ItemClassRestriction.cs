using System;

namespace Navislamia.Game.Models.Arcadia.Enums
{
    [Flags]
    public enum ItemClassRestriction : short
    {
        LimitDeva = 4,
        LimitAsura = 8,
        LimitGaia = 16,
    }
}
