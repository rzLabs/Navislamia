using System;

namespace Navislamia.Game.Models.Enums
{
    [Flags]
    public enum ItemClassRestriction : short
    {
        LimitDeva = 4,
        LimitAsura = 8,
        LimitGaia = 16,
    }
}
